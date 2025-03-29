using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WingetNexus.Data;
using WingetNexus.Data.DataStores;
using WingetNexus.Server.Mappers;
using WingetNexus.Server.Security;
using WingetNexus.Shared.Entities;
using Version = WingetNexus.Shared.Entities.Version;

namespace WingetNexus.Controllers.v2
{
    [Route("api/v2/[controller]")]
    [ApiController]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "IsAuthorized", AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class PackagesController : ControllerBase
    {
        private readonly ILogger<PackagesController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _env;
        private readonly IWingetAppDatastore _dataStore;

        public PackagesController(
                                  ILogger<PackagesController> logger, 
                                  IConfiguration configuration, 
                                  IHostEnvironment env, 
                                  IWingetAppDatastore dataStore)
        {
            _logger = logger;
            _configuration = configuration;
            _env = env;
            _dataStore = dataStore;
        }

        /// <summary>
        /// Get all packages with pagination
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet()]
        public async Task<ActionResult<IQueryable<Application>>> Get(
            [FromQuery] int? page,
            [FromQuery] int? pageSize,
            [FromQuery] string? filter,
            [FromQuery] string? orderby,
            [FromQuery] string? orderway)
        {
            var result = await _dataStore.GetAllApplicationsAsync(filter, page, pageSize, orderby, orderway);
            var packageCnt = await _dataStore.GetApplicationCountAsync(filter);

            Response.Headers.Append("X-Total-Count", packageCnt.ToString());

            return Ok(result);
        }

        // get all package count based on a filter
        [HttpGet("count")]
        public async Task<ActionResult<int>> GetTotalCount()
        {
            var packageCnt = await _dataStore.GetApplicationCountAsync();

            return Ok(packageCnt);
        }

        /// <summary>
        /// Get full package details by identifier
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        //[Authorize]
        //[Authorize(Policy = "get:package")]
        public async Task<ActionResult<Application>> GetPackage(string packageIdentifier)
        {
            var result = await _dataStore.GetApplicationByPackageIdentifierAsync(packageIdentifier);

            return Ok(result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="packageForm"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<Application>> Post([FromBody] Application packageForm)
        {
            var hasValidationErrors = false;
            string validationErrors = "";

            ValidateCreateForm(packageForm, ref hasValidationErrors, ref validationErrors);

            if (hasValidationErrors)
            {
                return StatusCode(500, validationErrors);
            }

            var identifier = $"{packageForm.Publisher.Name.Replace(" ", "")}.{packageForm.Name.Replace(" ", "")}";
            if (_dataStore.GetApplicationByPackageIdentifierAsync(identifier) != null)
            {
                return StatusCode(500, "Package identifier must be unique");
            }
            

            var package = new Application(identifier, packageForm.Name, packageForm.Publisher);

            //package.Versions = new List<Version>();

            //foreach (var versionForm in packageForm.Versions)
            //{
            //    var local = _context.Locales.FirstOrDefault(l => l.PackageLocale == versionForm.PackageLocale);
            //    if (local == null)
            //    {
            //        local = new Locale(versionForm.PackageLocale);
            //        _context.Locales.Add(local);
            //        try
            //        {
            //            await _context.SaveChangesAsync();
            //        }
            //        catch (Exception e)
            //        {
            //            _logger.LogError($"Error committing to the database: {e}");
            //            return StatusCode(500, "Database error");
            //        }
            //    }

            //    var version = new PackageVersion(versionForm.VersionCode, versionForm.PackageLocale, identifier, versionForm.ShortDescription);
            //    version.Installers = new List<Installer>();

            //    foreach (var installerForm in versionForm.Installers)
            //    {
            //        var installer = _dataStore.CreateInstaller(installerForm);
            //        version.Installers.Add(installer);
            //    }

            //    //var version = _dataStore.CreatePackageVersion(versionForm);

            //    package.Versions.Add(version);
            //}

            var newApp = await _dataStore.CreateApplicationAsync(package);

            return Ok(newApp);
        }

        



        /// <summary>
        /// 
        /// </summary>
        /// <param name="packageForm"></param>
        /// <param name="identifier"></param>
        /// <returns></returns>
        //[HttpPut("{identifier}/infos")]
        //public async Task<ActionResult<VersionDto>> Put([FromBody] NewPackageDto packageForm, string identifier)
        //{
        //    var hasValidationErrors = false;
        //    string validationErrors = "";
        //    Package package = null;

        //    //ValidateCreateForm(packageForm, ref hasValidationErrors, ref validationErrors);

        //    if (string.IsNullOrEmpty(identifier))
        //    {
        //        hasValidationErrors = true;
        //        validationErrors += "Package identifier is missing";
        //    }

        //    if (hasValidationErrors)
        //    {
        //        return StatusCode(500, validationErrors);
        //    }

        //    try
        //    {
        //        package = _context.Packages.FirstOrDefault(p => p.Identifier == identifier);
        //        if (package == null)
        //        {
        //            return StatusCode(204, "Package not found");
        //        }

        //        package.Name = packageForm.Name;
        //        package.Publisher = packageForm.Publisher;

        //        await _context.SaveChangesAsync();
        //        _logger.LogDebug("Updated to db");
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogDebug($"Error committing to the database: {e}");
        //        return StatusCode(500, "Database error");
        //    }

        //    return Ok(package);
        //}

        /// <summary>
        /// Delete specified package with cascading delete
        /// </summary>
        /// <param name="packageID"></param>
        /// <returns></returns>
        [HttpDelete("{packageID}")]
        //[Authorize]
        public ActionResult DeletePackage(string packageID)
        {
            _dataStore.DeleteApplicationAsync(packageID);

            return NoContent();
        }

        private void ValidateUpdateForm(Application packageForm, ref bool hasValidationErrors, ref string validationErrors)
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

            if (packageForm.Publisher == null)
            {
                hasValidationErrors = true;
                validationErrors += "No package publisher provided";
            }

            if (hasValidationErrors)
            {
                throw new Exception(validationErrors);
            }
        }

        private void ValidateCreateForm(Application packageForm, ref bool hasValidationErrors, ref string validationErrors)
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

            if (packageForm.Publisher == null)
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
