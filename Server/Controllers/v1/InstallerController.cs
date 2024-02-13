using k8s.KubeConfigModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WingetNexus.Controllers.v1;
using WingetNexus.Data;
using WingetNexus.Data.DataStores;
using WingetNexus.Server.Mappers;
using WingetNexus.Shared.Models.Db;
using WingetNexus.Shared.Models.Db.Objects;
using WingetNexus.Shared.Models.Dtos;
using static MudBlazor.CategoryTypes;

namespace WingetNexus.Server.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "IsAuthorized", AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class InstallerController : ControllerBase
    {
        private readonly WingetNexusContext _context;
        private readonly ILogger<PackagesController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _env;
        private readonly IWingetNexusDataStore _dataStore;

        public InstallerController(WingetNexusContext context, ILogger<PackagesController> logger, IConfiguration configuration, IHostEnvironment env, IWingetNexusDataStore dataStore)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
            _env = env;
            _dataStore = dataStore;
        }

        /// <summary>
        /// Get an installer based on th ID
        /// </summary>
        /// <param name="installerId"></param>
        /// <returns></returns>
        //[HttpGet("{installerId}")]
        //public ActionResult<InstallerDto> GetInstaller(int installerId)
        //{
        //    var installer = _context.Installers
        //        .Include(i => i.NestedInstallerFiles)
        //        .Include(i => i.Switches)
        //        .FirstOrDefault(i => i.Id == installerId);

        //    if (installer == null)
        //    {
        //        return StatusCode(204, $"Installer {installerId} not found");
        //    }

        //    var dto = InstallerMapper.InstallerToInstallerDto(installer);
        //    return Ok(dto);
        //}

        /// </summary>
        /// <param name="installerForm">parameters for the installer</param>
        /// <param name="versionId">id of version to add</param>
        /// <returns></returns>
        [HttpPost("{versionId}")]
        public async Task<ActionResult<VersionDto>> PostInstaller([FromBody] InstallerDto installerForm, int versionId)
        {
            var hasValidationErrors = false;
            string validationErrors = "";
            PackageVersion version = null;

            _logger.LogDebug($"Adding installer to version {versionId}");

            ValidateCreateForm(installerForm, ref hasValidationErrors, ref validationErrors);

            //if (string.IsNullOrEmpty(versionId))
            //{
            //    hasValidationErrors = true;
            //    validationErrors += "Version id identifier is missing";
            //}

            if (hasValidationErrors)
            {
                _logger.LogDebug($"Validation errors: {validationErrors}");
                return StatusCode(500, validationErrors);
            }

            try
            {
                version = _context.PackageVersions
                    .Include(p => p.Installers)
                    .Include(p=>p.Package)
                    .Include("Installers.Switches")
                    .FirstOrDefault(p => p.Id == versionId);

                if (version == null)
                {
                    _logger.LogDebug($"Version {versionId} not found");
                    return StatusCode(204, $"Version {versionId} not found");
                }

                var installer = _dataStore.CreateInstaller(installerForm);

                if (version.Installers != null)
                {
                    version.Installers.Add(installer);
                }
                else
                {
                    version.Installers = new List<Installer>() { installer };
                }

                try
                {
                    await _context.SaveChangesAsync();
                    _logger.LogDebug("Commited to db");
                }
                catch (Exception e)
                {
                    _logger.LogError($"Error committing to the database: {e}");
                    return StatusCode(500, "Database error");
                }

                var dto = VersionMapper.VersionToVersionDto(version);
                return Ok(dto);


            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding installer to version {versionId}: {ex.Message}");
                return StatusCode(500, $"Error adding installer to version {versionId}: {ex.Message}");
            }
        }

        /// <summary>
        /// Update an existing installer
        /// </summary>
        /// <param name="installerForm">parameters for the installer</param>
        /// <param name="installerId">id of installer to update</param>
        /// <returns></returns>
        [HttpPut("{installerId}")]
        public async Task<ActionResult<InstallerDto>> PutInstaller([FromBody] InstallerDto installerForm, int installerId)
        {
            var hasValidationErrors = false;
            string validationErrors = "";

            Installer installer = null;

            _logger.LogDebug($"Updating installer {installerId}");

            if (!ModelState.IsValid)
            {
                _logger.LogDebug($"Validation errors: {validationErrors}");
                return StatusCode(500, validationErrors);
            }

            ValidateUpdateForm(installerForm, ref hasValidationErrors, ref validationErrors);

            if (hasValidationErrors)
            {
                _logger.LogDebug($"Validation errors: {validationErrors}");
                return StatusCode(500, validationErrors);
            }

            try
            {
                installer = _context.Installers
                    .Include(i => i.NestedInstallerFiles)
                    .Include(i => i.Switches)
                    .Include(i => i.Version)
                    .FirstOrDefault(p => p.Id == installerId);
                if (installer == null)
                {
                    _logger.LogDebug($"Installer not found for id {installerId}");
                    return StatusCode(204, "Installer not found");
                }

                //update all fields of installerCnt with installerForm values
                installer.Architecture = installerForm.Architecture;
                installer.InstallerType = installerForm.InstallerType;
                installer.InstallerPath = installerForm.InstallerPath;
                installer.InstallerSha256 = installerForm.InstallerSha256;
                installer.Scope = installerForm.Scope;
                installer.Switches = installerForm.Switches.Select(s => new InstallerSwitch() { Parameter = s.Parameter, Value = s.Value }).ToList();

                if (installer.NestedInstallerFiles != null && installer.NestedInstallerFiles.Count >0)
                {
                    installer.NestedInstallerFiles = installerForm.NestedInstallerFiles.Select(n => new NestedInstallerFile() { RelativeFilePath = n.RelativeFilePath, PortableCommandAlias = n.PortableCommandAlias }).ToList();
                }
                else
                {
                    installer.NestedInstallerFiles = null;
                }

                var report = _context.Installers.Update(installer);

                installer = report.Entity;
                //if (report.Properties.Count > 0)
                //{
                //    await _context.SaveChangesAsync();
                //    _logger.LogDebug("Updated to db");
                //}
                //else
                //{
                //    _logger.LogDebug("No changes to db");
                //}

                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogWarning($"Error committing to the database: {e}");
                return StatusCode(500, "Database error");
            }

            return Ok(installer);
        }

        private void ValidateCreateForm(InstallerDto installerForm, ref bool hasValidationErrors, ref string validationErrors)
        {
            if (installerForm == null)
            {
                hasValidationErrors = true;
                validationErrors += "Installer form is missing";
            }
            else
            {
                if (installerForm.Switches == null)
                {
                    hasValidationErrors = true;
                    validationErrors += "Installer form is missing switches";
                }
                if (string.IsNullOrEmpty(installerForm.Architecture))
                {
                    hasValidationErrors = true;
                    validationErrors += "Installer architecture is missing";
                }
                if (string.IsNullOrEmpty(installerForm.InstallerSha256))
                {
                    hasValidationErrors = true;
                    validationErrors += "Installer SHA256 is missing";
                }
                if (string.IsNullOrEmpty(installerForm.InstallerPath))
                {
                    hasValidationErrors = true;
                    validationErrors += "Installer URL is missing";
                }
                if (string.IsNullOrEmpty(installerForm.InstallerType))
                {
                    hasValidationErrors = true;
                    validationErrors += "Installer type is missing";
                }
                if (string.IsNullOrEmpty(installerForm.Scope))
                {
                    hasValidationErrors = true;
                    validationErrors += "Installer scope is missing";
                }
                if (installerForm.InstallerType == "zip" && (installerForm.NestedInstallerFiles == null || installerForm.NestedInstallerFiles.Count == 0))
                {
                    hasValidationErrors = true;
                    validationErrors += "Nested installer files are missing";
                }
            }
        }

        private void ValidateUpdateForm(InstallerDto installerForm, ref bool hasValidationErrors, ref string validationErrors)
        {
            if (installerForm == null)
            {
                hasValidationErrors = true;
                validationErrors += "Installer form is missing";
            }
            else
            {
                if (installerForm.PackageVersionId == 0)
                {
                    hasValidationErrors = true;
                    validationErrors += "Version reference is missing";
                }
                if (installerForm.Switches == null)
                {
                    hasValidationErrors = true;
                    validationErrors += "Installer form is missing switches";
                }
                if(string.IsNullOrEmpty(installerForm.Architecture))
                        {
                    hasValidationErrors = true;
                    validationErrors += "Installer architecture is missing";
                }
                if (string.IsNullOrEmpty(installerForm.InstallerSha256))
                {
                    hasValidationErrors = true;
                    validationErrors += "Installer SHA256 is missing";
                }
                if (string.IsNullOrEmpty(installerForm.InstallerPath))
                {
                    hasValidationErrors = true;
                    validationErrors += "Installer URL is missing";
                }
                if (string.IsNullOrEmpty(installerForm.InstallerType))
                {
                    hasValidationErrors = true;
                    validationErrors += "Installer type is missing";
                }
                if (string.IsNullOrEmpty(installerForm.Scope))
                {
                    hasValidationErrors = true;
                    validationErrors += "Installer scope is missing";
                }
            }
        }

        /// <summary>
        /// Delete an existing installer
        /// </summary>
        /// <param name="installerId">ID of existing installer</param>
        /// <returns></returns>
        [HttpDelete("{installerId}")]
        public async Task<ActionResult<VersionDto>> DeleteInstaller(int installerId)
        {
            Installer installer = null;

            _logger.LogDebug($"Deleting installer {installerId}");

            try
            {
                installer = _context.Installers.FirstOrDefault(p => p.Id == installerId);
                if (installer == null)
                {
                    _logger.LogDebug($"Installer not found for id {installerId}");
                    return StatusCode(204, "Installer not found");
                }

                _context.Installers.Remove(installer);
                await _context.SaveChangesAsync();
                _logger.LogDebug("Deleted from db");
            }
            catch (Exception e)
            {
                _logger.LogWarning($"Error committing to the database: {e}");
                return StatusCode(500, "Database error");
            }

            return Ok(installer);
        }

        //private string ExtractNameFromCommandLine(string filename)
        //{
        //    var name = filename.Substring(filename.LastIndexOf('\\') + 1, filename.Length - filename.LastIndexOf('\\') - filename.LastIndexOf('.') - 1);
        //    return name;
        //}
    }
}
