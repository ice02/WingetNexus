using WingetNexus.Shared.Models.Dtos;
using WingetNexus.Data.DataStores;

namespace WingetNexus.Server.Controllers.v2
{
    [Route("api/v2/[controller]")]
    [ApiController]
    [Authorize(Policy = "IsAuthorized", AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class VersionsController : ControllerBase
    {
        private readonly ILogger<VersionsController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _env;
        private readonly IVersionDatastore _versionDatastore;
        private readonly IWingetAppDatastore _wingetAppDatastore;

        public VersionsController(
            ILogger<VersionsController> logger, 
            IConfiguration configuration, 
            IHostEnvironment env, 
            IVersionDatastore dataStore,
            IWingetAppDatastore wingetAppDatastore)
        {
            _logger = logger;
            _configuration = configuration;
            _env = env;
            _versionDatastore = dataStore;
            _wingetAppDatastore = wingetAppDatastore;
        }

        [HttpPost("{packageIdentifier}")]
        [ValidateAntiForgeryToken]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public async Task<ActionResult<VersionDto>> PostVersion([FromBody] VersionDto versionForm, string packageIdentifier)
        {
            var hasValidationErrors = false;
            string validationErrors = "";
            VersionDto version = null;

            _logger.LogDebug($"Creating new version");

            var pck = await _wingetAppDatastore.GetApplicationByPackageIdentifierAsync(packageIdentifier);
            if (pck == null)
            {
                _logger.LogDebug($"Package not found for identifier {packageIdentifier}");
                return StatusCode(204, "Application not found");
            }

            //ValidateCreateForm(packageForm, ref hasValidationErrors, ref validationErrors);

            if (hasValidationErrors)
            {
                _logger.LogDebug($"Validation errors: {validationErrors}");
                return StatusCode(500, validationErrors);
            }

            try
            {
                //versionForm.Application = pck;

                //if (versionForm.Installers != null && versionForm.Installers.Count > 0)
                //{
                //    if (version.Installers == null)
                //    {
                //        version.Installers = new List<Installer>();
                //    }

                //    foreach (var item in versionForm.Installers)
                //    {
                //        version.Installers.Add(_dataStore.CreateInstaller(item));
                //    }
                //}

                version = await _versionDatastore.CreateVersionAsync(versionForm);
                _logger.LogDebug("Version created");
            }
            catch (Exception e)
            {
                _logger.LogWarning($"Error creating new version: {e}");
                return StatusCode(500, "Database error");
            }

            return Ok(version);
        }

        /// <summary>
        /// Update a version based on it ID
        /// </summary>
        /// <param name="versionForm">values to update</param>
        /// <param name="versionId">ID of version to update</param>
        /// <returns></returns>
        [HttpPut("{versionId}")]
        [ValidateAntiForgeryToken]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public async Task<ActionResult<VersionDto>> PutVersion([FromBody] VersionDto versionForm, int versionId)
        {
            var hasValidationErrors = false;
            string validationErrors = "";

            _logger.LogDebug($"Updating version {versionId}");

            if (!ModelState.IsValid)
            {
                _logger.LogDebug($"Validation errors: {validationErrors}");
                return StatusCode(500, validationErrors);
            }

            ValidateUpdateForm(versionForm, ref hasValidationErrors, ref validationErrors);

            if (hasValidationErrors)
            {
                _logger.LogDebug($"Validation errors: {validationErrors}");
                return StatusCode(500, validationErrors);
            }

            VersionDto result = null;
            try
            {
                result = await _versionDatastore.UpdateVersionAsync(versionForm);
                if (result == null)
                {
                    _logger.LogDebug($"Version not found for id {versionId}");
                    return StatusCode(204, "Version not found");
                }

                // debug logs
                _logger.LogDebug($"Version {result.VersionNumber} for application {result.Application.PackageIdentifier} updated");
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, $"Error during update: {e.Message}");
                return StatusCode(500, "Update error");
            }

            return Ok(result);
        }

        [HttpDelete("{versionId}")]
        [ValidateAntiForgeryToken]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public async Task<ActionResult<bool>> DeleteVersion(int versionId)
        {
            //PackageVersion version = null;

            _logger.LogDebug($"Deleting version {versionId}");

            try
            {
                var result = await _versionDatastore.DeleteVersionAsync(versionId);
                _logger.LogDebug("Version deleted");
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, $"Error deleting version: {e}");
                return StatusCode(500, "Deleting error");
            }

            return NoContent();
        }


        private void ValidateUpdateForm(VersionDto versionForm, ref bool hasValidationErrors, ref string validationErrors)
        {
            hasValidationErrors = false;

            if (versionForm == null)
            {
                hasValidationErrors = true;
                validationErrors += "Version form is missing";
            }
            else
            {
                if (string.IsNullOrEmpty(versionForm.ApplicationIdentifier))
                {
                    hasValidationErrors = true;
                    validationErrors += "Package identifier is missing";
                }
                if (string.IsNullOrEmpty(versionForm.VersionNumber))
                {
                    hasValidationErrors = true;
                    validationErrors += "Version code is missing";
                }
                if (string.IsNullOrEmpty(versionForm.DefaultLocaleKey))
                {
                    hasValidationErrors = true;
                    validationErrors += "Package default locale is missing";
                }
                if (string.IsNullOrEmpty(versionForm.ShortDescription))
                {
                    hasValidationErrors = true;
                    validationErrors += "Short description is missing";
                }
                //if (versionForm.Installers != null && versionForm.Installers.Count > 0)
                //{
                //    foreach (var item in versionForm.Installers)
                //    {
                //        if (string.IsNullOrEmpty(item.Architecture))
                //        {
                //            hasValidationErrors = true;
                //            validationErrors += "Installer architecture is missing";
                //        }
                //        if (string.IsNullOrEmpty(item.InstallerSha256))
                //        {
                //            hasValidationErrors = true;
                //            validationErrors += "Installer SHA256 is missing";
                //        }
                //        if (string.IsNullOrEmpty(item.InstallerPath))
                //        {
                //            hasValidationErrors = true;
                //            validationErrors += "Installer URL is missing";
                //        }
                //        if (string.IsNullOrEmpty(item.InstallerType))
                //        {
                //            hasValidationErrors = true;
                //            validationErrors += "Installer type is missing";
                //        }
                //        if (string.IsNullOrEmpty(item.Scope))
                //        {
                //            hasValidationErrors = true;
                //            validationErrors += "Installer scope is missing";
                //        }
                //        if (item.Switches == null)
                //        {
                //            hasValidationErrors = true;
                //            validationErrors += "Installer form is missing switches";
                //        }

                //    }
                //}
            }
        }

        private void ValidateCreateForm(VersionDto versionForm, ref bool hasValidationErrors, ref string validationErrors)
        {

        }
    }
}
