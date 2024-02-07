using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.FeatureManagement;
using Newtonsoft.Json.Linq;
using WingetNexus.Data;
using WingetNexus.Shared.Models.Db;
using WingetNexus.Shared.Models.Winget;
using WingetNexus.WingetApi.Helpers;

namespace WingetNexus.WingetApi.Controllers.v1
{
    /// <summary>
    /// Api for winget client based on winget contract version 1.1
    /// </summary>
    [Route("api/v1.1/[controller]")]
    [ApiController]
    [IgnoreAntiforgeryToken]
    public class WingetController : ControllerBase
    {
        //contructor with dependency injection ilogger and datacontext
        private readonly WingetNexusContext _context;
        private readonly ILogger<WingetController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IFeatureManager _featureManager;

        public WingetController(ILogger<WingetController> logger, WingetNexusContext context, IConfiguration configuration, IFeatureManager featureManager)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
            _featureManager = featureManager;
        }

        [AllowAnonymous]
        [HttpGet("information")]
        public IActionResult Information()
        {
            if (_featureManager.IsEnabledAsync("RequireAuthentication").Result)
            {
                CertificateValidationHelper.ValidateAuthentication(Request, _logger);
            }

            // add to log all information from header
            foreach (var header in Request.Headers)
            {
                _logger.LogDebug($"{header.Key}: {header.Value}");
            }

            return new JsonResult(new
            {
                Data = new
                {
                    SourceIdentifier = _configuration["Reponame"],
                    ServerSupportedVersions = new[] { "1.0.0", "1.1.0" },
                    UnsupportedPackageMatchFields = new List<string>(),
                    RequiredPackageMatchFields = new List<string>(),
                    UnsupportedQueryParameters = new List<string>(),
                    RequiredQueryParameters = new List<string>(),
                }
            });
        }

        [HttpGet("packageManifests/{identifier}")]
        public async Task<IActionResult> GetPackageManifestBak(string identifier)
        {
            //var package = await _context.Packages
            //    .Include(p => p.Versions)
            //        .ThenInclude(v => v.Installers)
            //            .ThenInclude(i => i.NestedInstallerFiles)

            //    .FirstOrDefaultAsync(p => p.Identifier == identifier);

            var package = _context.Packages
                            .Include(p => p.Versions)
                            .Include("Versions.Installers")
                            .Include("Versions.Installers.NestedInstallerFiles")
                            .Include("Versions.Installers.Switches")
                            .FirstOrDefault(p => p.Identifier == identifier);

            if (package == null)
            {
                return NoContent();
            }

            var result = new Dictionary<string, object>
            {
                { "PackageIdentifier", package.Identifier }
            };

            var versions = new List<Dictionary<string, object>>();

            foreach (var version in package.Versions)
            {
                var installers = new List<Dictionary<string, object>>();

                foreach (var installer in version.Installers)
                {
                    var installerJson = new Dictionary<string, object>
                    {
                        { "Architecture", installer.Architecture },
                        { "InstallerType", installer.InstallerType },
                        { "InstallerUrl", Url.Action("GetInstallerFile", new { id = installer.Id }, Request.Scheme) },
                        { "InstallerSha256", installer.InstallerSha256 },
                        { "Scope", installer.Scope }
                    };

                    if (!string.IsNullOrEmpty(installer.NestedInstaller))
                    {
                        installerJson["NestedInstallerFiles"] = new List<Dictionary<string, object>>
                        {
                            new Dictionary<string, object> { { "RelativeFilePath", installer.NestedInstaller } }
                        };
                    }

                    if (!string.IsNullOrEmpty(installer.NestedInstallerType))
                    {
                        installerJson["NestedInstallerType"] = installer.NestedInstallerType;
                    }

                    var switches = new Dictionary<string, string>
                    {
                        { "Silent", installer.Switches.FirstOrDefault(p=>p.Parameter == "Silent")?.Value },
                        { "SilentWithProgress", installer.Switches.FirstOrDefault(p=>p.Parameter == "SilentWithProgress")?.Value },
                        { "Interactive", installer.Switches.FirstOrDefault(p=>p.Parameter == "Interactive")?.Value },
                        { "InstallLocation", installer.Switches.FirstOrDefault(p=>p.Parameter == "InstallLocation")?.Value },
                        { "Log", installer.Switches.FirstOrDefault(p=>p.Parameter == "Log")?.Value },
                        { "Upgrade", installer.Switches.FirstOrDefault(p=>p.Parameter == "Upgrade")?.Value },
                        { "Custom", installer.Switches.FirstOrDefault(p=>p.Parameter == "Custom")?.Value }
                    };

                    var nonemptySwitches = switches.Where(s => !string.IsNullOrEmpty(s.Value)).ToDictionary(s => s.Key, s => s.Value);

                    if (nonemptySwitches.Any())
                    {
                        installerJson["InstallerSwitches"] = nonemptySwitches;
                    }

                    installers.Add(installerJson);
                }

                var versionJson = new Dictionary<string, object>
                {
                    { "PackageVersion", version.VersionCode },
                    { "DefaultLocale", new Dictionary<string, string>
                        {
                            { "PackageLocale", "en-us" },
                            { "Publisher", package.Publisher },
                            { "PackageName", package.Name },
                            { "ShortDescription", version.ShortDescription }
                        }
                    },
                    { "Installers", installers }
                };

                versions.Add(versionJson);
            }

            result["Versions"] = versions;

            return Ok(result);
        }



        [HttpGet("packageManifests/{identifier}")]
        public IActionResult GetPackageManifest(string identifier)
        {
            if (_featureManager.IsEnabledAsync("RequireAuthentication").Result)
            {
                CertificateValidationHelper.ValidateAuthentication(Request, _logger);
            }

            var package = _context.Packages
                            .Include(p => p.Versions)
                            .Include("Versions.Installers")
                            .Include("Versions.Installers.NestedInstallerFiles")
                            .Include("Versions.Installers.Switches")
                            .FirstOrDefault(p => p.Identifier == identifier);
            
            if (package == null)
            {
                return NoContent();
            }

            var result = new ManifestExtended();
            var versionData = new List<ManifestVersion>();
            foreach (var version in package.Versions)
            {
                var data = new ManifestVersion()
                {
                    PackageVersion = version.VersionCode,
                    DefaultLocale = new ManifestDefaultLocal()
                    {
                        Moniker = version.Identifier,
                        PackageLocale = version.PackageLocale,
                        Publisher = package.Publisher,
                        PackageName = package.Name,
                        ShortDescription = version.ShortDescription
                    },
                    Installers = GetInstallerData(version),
                    Locales = new ManifestLocal[0]
                };

                // Only append version if there's at least one installer
                if (data.Installers.Any())
                {
                    versionData.Add(data);
                }
            }

            var output0 = package.GenerateOutput();
            var output = new { Data = new { PackageIdentifier = package.Identifier, Versions = versionData } };

            return Ok(output);
        }

        private ManifestInstaller[] GetInstallerData(PackageVersion version)
        {
            if (version.Installers == null)
            {
                return new ManifestInstaller[0];
            }

            var installerData = new List<ManifestInstaller>();
            foreach (var installer in version.Installers)
            {

                if (installer.Scope == "both")
                {
                    // If installer is for both user and machine, create two entries for each scope (user and machine) but use it with download url
                    foreach (var scope in new[] { "user", "machine" })
                    {
                        var installerPath = installer.InstallerPath;
                        if (installer.IsLocalPackage)
                            installerPath = $"{_configuration["InstallerPath"]}/api/v1/Files/{installer.InstallerPath}";

                        var data = new ManifestInstaller()
                        {
                            Architecture = installer.Architecture,
                            InstallerType = installer.InstallerType,
                            InstallerUrl = installerPath,
                            InstallerSha256 = installer.InstallerSha256,
                            Scope = scope,
                            InstallerSwitches = GetInstallerSwitches(installer)
                        };

                        if (installer.InstallerType == "zip")
                        {
                            data.NestedInstallerType = installer.NestedInstallerType;
                            data.NestedInstallerFiles = GetNestedInstallerData(installer);
                        }
                        installerData.Add(data);
                    }
                }
                else
                {
                    var installerPath = installer.InstallerPath;
                    if (installer.IsLocalPackage)
                        installerPath = $"{_configuration["InstallerPath"]}/api/v1/Files/{installer.InstallerPath}";

                    var data = new ManifestInstaller()
                    {
                        Architecture = installer.Architecture,
                        InstallerType = installer.InstallerType,
                        InstallerUrl = installerPath,
                        InstallerSha256 = installer.InstallerSha256,
                        Scope = installer.Scope,
                        InstallerSwitches = GetInstallerSwitches(installer)
                    };

                    if (installer.InstallerType == "zip")
                    {
                        data.NestedInstallerType = installer.NestedInstallerType;
                        data.NestedInstallerFiles = GetNestedInstallerData(installer);
                    }
                    installerData.Add(data);
                }

            }

            return installerData.ToArray();
        }

        private ManifestNestedInstallerFile[] GetNestedInstallerData(Installer installer)
        {
            var nestedInstallerData = new List<ManifestNestedInstallerFile>();
            if (installer != null && installer.NestedInstallerFiles != null)
            {
                foreach (var nestedInstallerFile in installer.NestedInstallerFiles)
                {
                    var data = new ManifestNestedInstallerFile()
                    {
                        RelativeFilePath = nestedInstallerFile.RelativeFilePath,
                        PortableCommandAlias = nestedInstallerFile.PortableCommandAlias
                    };
                    nestedInstallerData.Add(data);
                }
            }
            return nestedInstallerData.ToArray();
        }

        private Dictionary<string, string> GetInstallerSwitches(Installer installer)
        {
            Dictionary<string, string> switches = new Dictionary<string, string>();
            if (installer.Switches == null)
            {
                return null;
            }
           
            foreach (var sw in installer.Switches)
    {
                switches[sw.Parameter] = sw.Value;
            }
            return switches;
        }

        [HttpPost("manifestSearch")]
        public async Task<IActionResult> ManifestSearch([FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] ManifestSearchRequest request_data)
        {
            if (_featureManager.IsEnabledAsync("RequireAuthentication").Result)
            {
                CertificateValidationHelper.ValidateAuthentication(Request, _logger);
            }

            _logger.LogDebug(request_data.ToString());

            var maximum_results = request_data.MaximumResults ?? 100;
            var fetch_all_manifests = request_data.FetchAllManifests;

            var query = request_data.Query;
            string keyword = null;
            string match_type = null;
            if (query != null)
            {
                keyword = query.KeyWord;
                match_type = query.MatchType;
            }

            var inclusions = request_data.Inclusions;
            string package_match_field = null;
            SearchRequestMatch request_match = null;
            if (inclusions != null)
            {
                package_match_field = inclusions[0].PackageMatchField;
                request_match = inclusions[0].RequestMatch;
                if (query == null)
                {
                    keyword = request_match.KeyWord;
                    match_type = request_match.MatchType;
                }
            }

            var filters = request_data.Filters;
            string package_match_field_filter = null;
            SearchRequestMatch request_match_filter = null;
            string keyword_filter = null;
            string match_type_filter = null;
            if (filters != null)
            {
                package_match_field_filter = filters[0].PackageMatchField;
                request_match_filter = filters[0].RequestMatch;
                keyword_filter = request_match_filter.KeyWord;
                match_type_filter = request_match_filter.MatchType;
            }

            var results = new ManifestSearchResponse();
            var packages = new List<Package>();
            if (keyword != null && match_type != null)
            {
                var ctx_request = _context.Packages
                    .Include(p => p.Versions)
                    .Include("Versions.Installers")
                    .Include("Versions.Installers.Switches")
                    .Include("Versions.Installers.NestedInstallerFiles");

                if (match_type == "Exact")
                {
                    var packages_query = ctx_request.Where(p => p.Identifier == keyword);
                    if (packages_query.FirstOrDefault() == null)
                    {
                        _logger.LogDebug("No package found with identifier, searching for package name");
                        packages_query = ctx_request.Where(p => p.Name == keyword);
                    }
                    packages = packages_query.ToList();
                }
                else if (match_type == "Partial" || match_type == "Substring")
                {
                    var packages_query = ctx_request.Where(p => p.Name.Contains(keyword));
                    if (packages_query.FirstOrDefault() == null)
                    {
                        _logger.LogDebug("No package found with name, searching for package identifier");
                        packages_query = ctx_request.Where(p => p.Identifier.Contains(keyword));
                    }
                    packages = packages_query.ToList();
                }
                else
                {
                    return NoContent();
                }

                packages = packages.Take(maximum_results).ToList();
                
            }

            if (!packages.Any())
            {
                return NoContent();
            }

            var output_data = new List<JObject>();
            results.Data = new List<ManifestSearchResponseItem>();
            foreach (var package in packages)
            {
                //if (package.Versions.Any())
                //{
                //    output_data.Add(package.GenerateOutputManifestSearch());
                //}
                results.Data.Add(new ManifestSearchResponseItem()
                {
                    PackageIdentifier = package.Identifier,
                    PackageName = package.Name,
                    Publisher = package.Publisher,
                    Versions = package.Versions.Where(p => p.Installers.Any()).Select(v => new SearchVersions()
                    {
                        PackageVersion = v.VersionCode,
                        //Installers = v.Installers?.ToList(),
                        //Channel = v.Channel
                    }).ToArray()
                });
            }

            _logger.LogDebug($"Found {results.Data.Count} results");

            return Ok(results);

            //var output = new JObject(new JProperty("Data", new JArray(output_data)));
            //_logger.LogDebug(output.ToString());
            //return new JsonResult(output);
        }
    }
}
