using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WingetNexus.Data;
using WingetNexus.Data.DataStores;
using WingetNexus.Server.Mappers;
using WingetNexus.Shared.Models.Db;
using WingetNexus.Shared.Models.Db.Objects;
using WingetNexus.Shared.Models.Dtos;

namespace WingetNexus.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [ValidateAntiForgeryToken]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Authorize(policy: "IsAuthorized")]
    public class PackagesController : ControllerBase
    {
        private readonly WingetNexusContext _context;
        private readonly ILogger<PackagesController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _env;
        private readonly IWingetNexusDataStore _dataStore;

        public PackagesController(WingetNexusContext context, 
                                  ILogger<PackagesController> logger, 
                                  IConfiguration configuration, 
                                  IHostEnvironment env, 
                                  IWingetNexusDataStore dataStore)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
            _env = env;
            _dataStore = dataStore;
        }

        /// <summary>
        /// Get all packages
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IQueryable<PackageDto>>> Get()
        {
            var packages = _context.Packages
                .Include(p => p.Versions)
                .AsQueryable();

            var dto = PackageMapper.ProjectToDto(packages);

            return Ok(dto);
        }

        /// <summary>
        /// Get full package details by identifier
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        //[Authorize]
        //[Authorize(Policy = "get:package")]
        public async Task<ActionResult<PackageDto>> GetPackage(string id)
        {
            var package = await _context.Packages
                .Include(p => p.Versions)
                .Include("Versions.Installers")
                .Include("Versions.DefaultLocale")
                .Include("Versions.Installers.NestedInstallerFiles")
                .Include("Versions.Installers.Switches")
                .FirstAsync(p => p.Identifier == id);

            var dto = PackageMapper.PackageToPackageDto(package);

            return Ok(dto);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="packageForm"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<PackageDto>> Post([FromBody] PackageDto packageForm)
        {
            var hasValidationErrors = false;
            string validationErrors = "";

            ValidateCreateForm(packageForm, ref hasValidationErrors, ref validationErrors);

            if (hasValidationErrors)
            {
                return StatusCode(500, validationErrors);
            }

            var identifier = $"{packageForm.Publisher.Replace(" ", "")}.{packageForm.Name.Replace(" ", "")}";
            if (_context.Packages.Any(p => p.Identifier == identifier))
            {
                return StatusCode(500, "Package identifier must be unique");
            }

            var package = new Package(identifier, packageForm.Name, packageForm.Publisher);

            package.Versions = new List<PackageVersion>();

            foreach (var versionForm in packageForm.Versions)
            {
                var local = _context.Locales.FirstOrDefault(l => l.PackageLocale == versionForm.PackageLocale);
                if (local == null)
                {
                    local = new Locale(versionForm.PackageLocale);
                    _context.Locales.Add(local);
                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"Error committing to the database: {e}");
                        return StatusCode(500, "Database error");
                    }
                }

                var version = new PackageVersion(versionForm.VersionCode, versionForm.PackageLocale, identifier, versionForm.ShortDescription);
                version.Installers = new List<Installer>();

                foreach (var installerForm in versionForm.Installers)
                {
                    var installer = _dataStore.CreateInstaller(installerForm);
                    version.Installers.Add(installer);
                }

                //var version = _dataStore.CreatePackageVersion(versionForm);

                package.Versions.Add(version);
            }

            try
            {
                _context.Packages.Add(package);
                await _context.SaveChangesAsync();
                _logger.LogDebug("Commited to db");
            }
            catch (Exception e)
            {
                _logger.LogError($"Error committing to the database: {e}");
                return StatusCode(500, "Database error");
            }

            var dto = PackageMapper.PackageToPackageDto(package);

            return Ok(dto);
        }

        



        /// <summary>
        /// 
        /// </summary>
        /// <param name="packageForm"></param>
        /// <param name="identifier"></param>
        /// <returns></returns>
        [HttpPut("{identifier}/infos")]
        public async Task<ActionResult<VersionDto>> Put([FromBody] NewPackageDto packageForm, string identifier)
        {
            var hasValidationErrors = false;
            string validationErrors = "";
            Package package = null;

            //ValidateCreateForm(packageForm, ref hasValidationErrors, ref validationErrors);

            if (string.IsNullOrEmpty(identifier))
            {
                hasValidationErrors = true;
                validationErrors += "Package identifier is missing";
            }

            if (hasValidationErrors)
            {
                return StatusCode(500, validationErrors);
            }

            try
            {
                package = _context.Packages.FirstOrDefault(p => p.Identifier == identifier);
                if (package == null)
                {
                    return StatusCode(204, "Package not found");
                }

                package.Name = packageForm.Name;
                package.Publisher = packageForm.Publisher;

                await _context.SaveChangesAsync();
                _logger.LogDebug("Updated to db");
            }
            catch (Exception e)
            {
                _logger.LogDebug($"Error committing to the database: {e}");
                return StatusCode(500, "Database error");
            }

            return Ok(package);
        }

        /// <summary>
        /// Delete specified package with cascading delete
        /// </summary>
        /// <param name="packageID"></param>
        /// <returns></returns>
        [HttpDelete("{packageID}")]
        //[Authorize]
        public ActionResult DeletePackage(string packageID)
        {
            var package = _context.Packages.FirstOrDefault(p => p.Identifier == packageID);

            if (package == null)
            {
                return NotFound();
            }

            try
            {
                _context.Packages.Remove(package);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"ERROR: {ex.Message}");
            }

            return NoContent();
        }

        private void ValidateUpdateForm(PackageDto packageForm, ref bool hasValidationErrors, ref string validationErrors)
        {
            if (packageForm == null)
            {
                hasValidationErrors = true;
                validationErrors += "No package form data provided";
                return;
            }

            if (string.IsNullOrEmpty(packageForm.Name))
            {
                hasValidationErrors = true;
                validationErrors += "No package name provided";
            }

            if (string.IsNullOrEmpty(packageForm.Publisher))
            {
                hasValidationErrors = true;
                validationErrors += "No package publisher provided";
            }

            if (hasValidationErrors)
            {
                throw new Exception(validationErrors);
            }
        }

        private void ValidateCreateForm(PackageDto packageForm, ref bool hasValidationErrors, ref string validationErrors)
        {
            if (packageForm == null)
            {
                hasValidationErrors = true;
                validationErrors += "No package form data provided";
                return;
            }

            if (string.IsNullOrEmpty(packageForm.Name))
            {
                hasValidationErrors = true;
                validationErrors += "No package name provided";
            }

            if (string.IsNullOrEmpty(packageForm.Publisher))
            {
                hasValidationErrors = true;
                validationErrors += "No package publisher provided";
            }

            //try
            //{
            //    var cnt = _context.Packages.Count(p=>p.Identifier == packageForm.Identifier);
            //    if (cnt > 0)
            //    {
            //        hasValidationErrors = true;
            //        validationErrors += "Identifier (publisher.name) must be unique";
            //    }
            //}
            //catch (Exception e)
            //{
            //    _logger.LogDebug($"Error accessing to the database: {e}");
            //}

            if (hasValidationErrors)
            {
                throw new Exception(validationErrors);
            }
        }

    }
}
