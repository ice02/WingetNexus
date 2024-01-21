using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WingetNexus.Controllers.v1;
using WingetNexus.Data;
using WingetNexus.Shared.Models.Dtos;
using Microsoft.IdentityModel.Tokens;
using WingetNexus.Shared.Models.Db;
using WingetNexus.Data.DataStores;
using WingetNexus.Server.Mappers;
using Microsoft.EntityFrameworkCore;

namespace WingetNexus.Server.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(Policy = "IsAuthorized", AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class VersionsController : ControllerBase
    {
        private readonly WingetNexusContext _context;
        private readonly ILogger<PackagesController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _env;
        private readonly IWingetNexusDataStore _dataStore;

        public VersionsController(WingetNexusContext context, ILogger<PackagesController> logger, IConfiguration configuration, IHostEnvironment env, IWingetNexusDataStore dataStore)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
            _env = env;
            _dataStore = dataStore;
        }

        [HttpPost("{packageIdentifier}")]
        [ValidateAntiForgeryToken]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public async Task<ActionResult<Package>> PostVersion([FromBody] VersionDto versionForm, string packageIdentifier)
        {
            var hasValidationErrors = false;
            string validationErrors = "";
            PackageVersion version = null;

            _logger.LogDebug($"Creating new version");

            var pck = _context.Packages.FirstOrDefault(p=>p.Identifier == packageIdentifier);
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
                version = new PackageVersion
                {
                    ShortDescription = versionForm.ShortDescription,
                    VersionCode = versionForm.VersionCode,
                    Package = pck,
                    Identifier = versionForm.Identifier,
                    PackageLocale = versionForm.PackageLocale
                };

                if (versionForm.Installers != null && versionForm.Installers.Count > 0)
                {
                    if (version.Installers == null)
                    {
                        version.Installers = new List<Installer>();
                    }

                    foreach (var item in versionForm.Installers)
                    {
                        version.Installers.Add(_dataStore.CreateInstaller(item));
                    }
                }

                _context.PackageVersions.Add(version);
                await _context.SaveChangesAsync();
                _logger.LogDebug("Saved to db");
            }
            catch (Exception e)
            {
                _logger.LogWarning($"Error committing to the database: {e}");
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
        public async Task<ActionResult<Package>> PutVersion([FromBody] VersionDto versionForm, int versionId)
        {
            var hasValidationErrors = false;
            string validationErrors = "";
            PackageVersion version = null;

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

            try
            {
                version = _context.PackageVersions
                    .Include(p => p.Installers)
                    //.Include(p => p.Locales)
                    .Include(p => p.DefaultLocale)
                    .Include(p => p.Package)
                    .FirstOrDefault(p => p.Id == versionId);

                if (version == null)
                {
                    _logger.LogDebug($"Version not found for id {versionId}");
                    return StatusCode(204, "Version not found");
                }

                version.Channel = versionForm.Channel;
                version.Identifier = versionForm.Identifier;
                version.PackageLocale = versionForm.PackageLocale;
                version.ShortDescription = versionForm.ShortDescription;
                version.VersionCode = versionForm.VersionCode;

                var report = _context.PackageVersions.Update(version);
                version = report.Entity;

                //TODO: report changes for debugging


                await _context.SaveChangesAsync();
                _logger.LogDebug("Updated to db");
            }
            catch (Exception e)
            {
                _logger.LogWarning($"Error committing to the database: {e}");
                return StatusCode(500, "Database error");
            }

            return Ok(version);
        }

        [HttpDelete("{versionId}")]
        [ValidateAntiForgeryToken]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public async Task<ActionResult<Package>> DeleteVersion(int versionId)
        {
            PackageVersion version = null;

            _logger.LogDebug($"Deleting version {versionId}");

            try
            {
                version = _context.PackageVersions
                    .FirstOrDefault(p => p.Id == versionId);

                if (version == null)
                {
                    _logger.LogDebug($"Version not found for id {versionId}");
                    return StatusCode(204, "Version not found");
                }

                _context.PackageVersions.Remove(version);

                await _context.SaveChangesAsync();
                _logger.LogDebug("Deleted from db");
            }
            catch (Exception e)
            {
                _logger.LogWarning($"Error committing to the database: {e}");
                return StatusCode(500, "Database error");
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
                if (string.IsNullOrEmpty(versionForm.Identifier))
                {
                    hasValidationErrors = true;
                    validationErrors += "Package identifier is missing";
                }
                if (string.IsNullOrEmpty(versionForm.VersionCode))
                {
                    hasValidationErrors = true;
                    validationErrors += "Version code is missing";
                }
                if (string.IsNullOrEmpty(versionForm.PackageLocale))
                {
                    hasValidationErrors = true;
                    validationErrors += "Package locale is missing";
                }
                if (string.IsNullOrEmpty(versionForm.ShortDescription))
                {
                    hasValidationErrors = true;
                    validationErrors += "Short description is missing";
                }
                if (versionForm.Installers != null && versionForm.Installers.Count > 0)
                {
                    foreach (var item in versionForm.Installers)
                    {
                        if (string.IsNullOrEmpty(item.Architecture))
                        {
                            hasValidationErrors = true;
                            validationErrors += "Installer architecture is missing";
                        }
                        if (string.IsNullOrEmpty(item.InstallerSha256))
                        {
                            hasValidationErrors = true;
                            validationErrors += "Installer SHA256 is missing";
                        }
                        if (string.IsNullOrEmpty(item.InstallerPath))
                        {
                            hasValidationErrors = true;
                            validationErrors += "Installer URL is missing";
                        }
                        if (string.IsNullOrEmpty(item.InstallerType))
                        {
                            hasValidationErrors = true;
                            validationErrors += "Installer type is missing";
                        }
                        if (string.IsNullOrEmpty(item.Scope))
                        {
                            hasValidationErrors = true;
                            validationErrors += "Installer scope is missing";
                        }
                        if (item.Switches == null)
                        {
                            hasValidationErrors = true;
                            validationErrors += "Installer form is missing switches";
                        }

                    }
                }
            }
        }

        private void ValidateCreateForm(VersionDto versionForm, ref bool hasValidationErrors, ref string validationErrors)
        {

        }
    }
}
