using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.FeatureManagement;
using Newtonsoft.Json.Linq;
using WingetNexus.Data;
using WingetNexus.Shared.Models.Winget._1._7;
using WingetNexus.WingetApi.Helpers;

namespace WingetNexus.WingetApi.Controllers.v1._7
{
    /// <summary>
    /// Api for winget client based on winget contract version 1.7
    /// </summary>
    [Route("api/v1.7/[controller]")]
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

        //TODO LIST 
        // "Channel": {
        //  "type": [ "string", "null" ],
        //  "minLength": 1,
        //  "maxLength": 16,
        //  "description": "The distribution channel"
        //},
        // installer un package portable public en mode verbose et voir les difference dans le payload
        // Dependencies (windowsFeatures, windowsLibraries, packageDependencies, externalDependencies)

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

            var info = new InformationResponseSchema()
            {
                Data = new InformationSchema()
                {
                    SourceIdentifier = _configuration["Reponame"],
                    ServerSupportedVersions = new ServerSupportedVersions() { "1.0.0", "1.1.0", "1.4.0", "1.7.0" },
                    UnsupportedPackageMatchFields = new PackageMatchFieldArray(),
                    RequiredPackageMatchFields = new PackageMatchFieldArray(),
                    UnsupportedQueryParameters = new QueryParameterArray() ,
                    RequiredQueryParameters = new QueryParameterArray(),
                    Authentication = new Authentication()
                    {
                        AuthenticationType = AuthenticationType.None ,
                        MicrosoftEntraIdAuthenticationInfo = new MicrosoftEntraIdAuthenticationInfo()
                        {
                            Resource = "Resource",
                            Scope = "test"
                        }
                    }
                }
            };

            //if (_featureManager.IsEnabledAsync("RequireAuthentication").Result)
            //{
            //    info.Data.Authentication.AuthenticationType = AuthenticationType.MicrosoftEntraId;
            //}

            return Ok(info);

            //return new JsonResult(new
            //{
            //    Data = new
            //    {
            //        SourceIdentifier = _configuration["Reponame"],
            //        ServerSupportedVersions = new[] { "1.0.0", "1.1.0", "1.4.0", "1.7.0" },
            //        UnsupportedPackageMatchFields = new List<string>(),
            //        RequiredPackageMatchFields = new List<string>(),
            //        UnsupportedQueryParameters = new List<string>(),
            //        RequiredQueryParameters = new List<string>(),
            //        Authentication = new
            //        {
            //            AuthenticationType = "microsoftEntraId",
                        
            //            MicrosoftEntraIdAuthenticationInfo = new
            //            {
            //                Resource = "Resource",
            //                Scope = "test"
            //            }
            //        }
            //    }
                
            //});
        }

        //[HttpGet("packageManifests/{identifier}")]
        //public async Task<IActionResult> GetPackageManifestBak(string identifier)
        //{
        //    //var package = await _context.Packages
        //    //    .Include(p => p.Versions)
        //    //        .ThenInclude(v => v.Installers)
        //    //            .ThenInclude(i => i.NestedInstallerFiles)

        //    //    .FirstOrDefaultAsync(p => p.Identifier == identifier);

        //    var package = _context.Packages
        //                    .Include(p => p.Versions)
        //                    .Include("Versions.Installers")
        //                    .Include("Versions.Installers.NestedInstallerFiles")
        //                    .Include("Versions.Installers.Switches")
        //                    .FirstOrDefault(p => p.Identifier == identifier);

        //    if (package == null)
        //    {
        //        return NoContent();
        //    }

        //    var result = new Dictionary<string, object>
        //    {
        //        { "PackageIdentifier", package.Identifier }
        //    };

        //    var versions = new List<Dictionary<string, object>>();

        //    foreach (var version in package.Versions)
        //    {
        //        var installers = new List<Dictionary<string, object>>();

        //        foreach (var installer in version.Installers)
        //        {
        //            var installerJson = new Dictionary<string, object>
        //            {
        //                { "Architecture", installer.Architecture },
        //                { "InstallerType", installer.InstallerType },
        //                { "InstallerUrl", Url.Action("GetInstallerFile", new { id = installer.Id }, Request.Scheme) },
        //                { "InstallerSha256", installer.InstallerSha256 },
        //                { "Scope", installer.Scope }
        //            };

        //            if (!string.IsNullOrEmpty(installer.NestedInstaller))
        //            {
        //                installerJson["NestedInstallerFiles"] = new List<Dictionary<string, object>>
        //                {
        //                    new Dictionary<string, object> { { "RelativeFilePath", installer.NestedInstaller } }
        //                };
        //            }

        //            if (!string.IsNullOrEmpty(installer.NestedInstallerType))
        //            {
        //                installerJson["NestedInstallerType"] = installer.NestedInstallerType;
        //            }

        //            var switches = new Dictionary<string, string>
        //            {
        //                { "Silent", installer.Switches.FirstOrDefault(p=>p.Parameter == "Silent")?.Value },
        //                { "SilentWithProgress", installer.Switches.FirstOrDefault(p=>p.Parameter == "SilentWithProgress")?.Value },
        //                { "Interactive", installer.Switches.FirstOrDefault(p=>p.Parameter == "Interactive")?.Value },
        //                { "InstallLocation", installer.Switches.FirstOrDefault(p=>p.Parameter == "InstallLocation")?.Value },
        //                { "Log", installer.Switches.FirstOrDefault(p=>p.Parameter == "Log")?.Value },
        //                { "Upgrade", installer.Switches.FirstOrDefault(p=>p.Parameter == "Upgrade")?.Value },
        //                { "Custom", installer.Switches.FirstOrDefault(p=>p.Parameter == "Custom")?.Value }
        //            };

        //            var nonemptySwitches = switches.Where(s => !string.IsNullOrEmpty(s.Value)).ToDictionary(s => s.Key, s => s.Value);

        //            if (nonemptySwitches.Any())
        //            {
        //                installerJson["InstallerSwitches"] = nonemptySwitches;
        //            }

        //            installers.Add(installerJson);
        //        }

        //        var versionJson = new Dictionary<string, object>
        //        {
        //            { "PackageVersion", version.VersionCode },
        //            { "DefaultLocale", new Dictionary<string, string>
        //                {
        //                    { "PackageLocale", "en-us" },
        //                    { "Publisher", package.Publisher },
        //                    { "PackageName", package.Name },
        //                    { "ShortDescription", version.ShortDescription }
        //                }
        //            },
        //            { "Installers", installers }
        //        };

        //        versions.Add(versionJson);
        //    }

        //    result["Versions"] = versions;

        //    return Ok(result);
        //}


        [HttpGet("packageManifests/{identifier}")]
        public IActionResult GetPackageManifest(string identifier)
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

            //get version from query string
            var queryVersion = Request.Query["Version"].ToString();

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

            var versionData = new List<Versions>();
            foreach (var version in package.Versions.Where(p=>p.VersionCode == queryVersion))
            {
                var data = new Versions()
                {
                    PackageVersion = version.VersionCode,
                    DefaultLocale = new DefaultLocale()
                    {
                        Moniker = version.Identifier,
                        PackageLocale = version.PackageLocale,
                        Publisher = package.Publisher,
                        PackageName = package.Name,
                        ShortDescription = version.ShortDescription
                    },
                    Channel = "stable", //version.Channel, 
                    Installers = GetInstallerData(version),
                    //Locales = new ManifestLocal[0]
                };

                // Only append version if there's at least one installer
                if (data.Installers.Any())
                {
                    versionData.Add(data);
                }
            }

            //var output0 = package.GenerateOutput();
            //var output = new { Data = new { PackageIdentifier = package.Identifier, Versions = versionData } };
            ManifestSingleResponseSchema output = new ManifestSingleResponseSchema()
            {
                Data = new ManifestSchema()
                {
                    PackageIdentifier = package.Identifier,
                    Versions = versionData
                }
            };

            Response.Headers.Add("Version", _configuration["CurrentVersion"].ToString());

            return Ok(output);
        }

        private List<Installer> GetInstallerData(Shared.Models.Db.PackageVersion version)
        {
            if (version.Installers == null)
            {
                return new List<Installer>();
            }

            var installerData = new List<Installer>();
            foreach (var installer in version.Installers)
            {

                if (installer.Scope == "both")
                {
                    // If installer is for both user and machine, create two entries for each scope (user and machine) but use it with download url
                    foreach (var scope in new[] { "user", "machine" })
                    {
                        AddManifestToInstaller(installerData, installer, scope);
                    }
                }
                else
                {
                    AddManifestToInstaller(installerData, installer, installer.Scope);
                }

            }

            return installerData;
        }

        private void AddManifestToInstaller(List<Installer> installerData, Shared.Models.Db.Installer installer, string scope)
        {
            var installerPath = installer.InstallerPath;
            if (installer.IsLocalPackage)
                installerPath = $"{_configuration["BaseUrl"]}/api/v1.7/Files/{installer.InstallerPath}";

            var data = new Installer()
            {
                Architecture = Enum.Parse<Architecture>(installer.Architecture),
                InstallerType = Enum.Parse<InstallerType>(installer.InstallerType),
                InstallerUrl = installerPath,
                InstallerSha256 = installer.InstallerSha256,
                Scope = Enum.Parse<Scope>(scope),
                InstallerSwitches = GetInstallerSwitches(installer)
            };

            if (installer.InstallerType == "zip")
            {
                // bug for now
                //data.NestedInstallerType = "portable";//installer.NestedInstallerType;
                data.NestedInstallerFiles = GetNestedInstallerData(installer);
            }
            installerData.Add(data);
        }

        private NestedInstallerFiles GetNestedInstallerData(Shared.Models.Db.Installer installer)
        {
            var nestedInstallerData = new NestedInstallerFiles();
            if (installer != null && installer.NestedInstallerFiles != null)
            {
                foreach (var nestedInstallerFile in installer.NestedInstallerFiles)
                {
                    var data = new NestedInstallerFile()
                    {
                        RelativeFilePath = nestedInstallerFile.RelativeFilePath,
                        PortableCommandAlias = nestedInstallerFile.PortableCommandAlias
                    };
                    nestedInstallerData.Add(data);
                }
            }
            return nestedInstallerData;
        }

        private InstallerSwitches GetInstallerSwitches(Shared.Models.Db.Installer installer)
        {
            InstallerSwitches switches = new InstallerSwitches();
            if (installer.Switches == null)
            {
                return null;
            }
           
            switches.Upgrade = installer.Switches.FirstOrDefault(p=>p.Parameter == "Upgrade")?.Value;
            switches.Silent = installer.Switches.FirstOrDefault(p=>p.Parameter == "Silent")?.Value;
            switches.SilentWithProgress = installer.Switches.FirstOrDefault(p=>p.Parameter == "SilentWithProgress")?.Value;
            switches.Interactive = installer.Switches.FirstOrDefault(p=>p.Parameter == "Interactive")?.Value;
            switches.InstallLocation = installer.Switches.FirstOrDefault(p=>p.Parameter == "InstallLocation")?.Value;
            switches.Log = installer.Switches.FirstOrDefault(p=>p.Parameter == "Log")?.Value;
            switches.Custom = installer.Switches.FirstOrDefault(p=>p.Parameter == "Custom")?.Value;

            return switches;
        }

        [HttpPost("manifestSearch")]
        public async Task<IActionResult> ManifestSearch([FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] ManifestSearchRequestSchema request_data)
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

            _logger.LogDebug(request_data.ToString());

            var maximum_results = request_data.MaximumResults > 0 ? request_data.MaximumResults : 100;
            var fetch_all_manifests = request_data.FetchAllManifests;

            var query = request_data.Query;
            string keyword = null;
            string match_type = null;
            if (query != null)
            {
                keyword = query.KeyWord;
                match_type = query.MatchType.ToString();
            }

            var inclusions = request_data.Inclusions;
            string package_match_field = null;
            SearchRequestMatch request_match = null;
            if (inclusions != null)
            {
                package_match_field = inclusions[0].PackageMatchField.ToString();
                request_match = inclusions[0].RequestMatch;
                if (query == null)
                {
                    keyword = request_match.KeyWord;
                    match_type = request_match.MatchType.ToString();
                }
            }

            var filters = request_data.Filters;
            string package_match_field_filter = null;
            SearchRequestMatch request_match_filter = null;
            string keyword_filter = null;
            string match_type_filter = null;
            if (filters != null)
            {
                package_match_field_filter = filters[0].PackageMatchField.ToString();
                request_match_filter = filters[0].RequestMatch;
                keyword_filter = request_match_filter.KeyWord;
                match_type_filter = request_match_filter.MatchType.ToString();
            }

            var results = new List<ManifestSearchResponseSchema>();
            var packages = new List<Shared.Models.Db.Package>();
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
            results = new List<ManifestSearchResponseSchema>();
            foreach (var package in packages)
            {
                //if (package.Versions.Any())
                //{
                //    output_data.Add(package.GenerateOutputManifestSearch());
                //}
                results.Add(new ManifestSearchResponseSchema()
                {
                    PackageIdentifier = package.Identifier,
                    PackageName = package.Name,
                    Publisher = package.Publisher,
                    Versions = package.Versions.Where(p => p.Installers.Any()).Select(v => new ManifestSearchVersionSchema()
                    {
                        PackageVersion = v.VersionCode,
                        //Installers = v.Installers?.ToList(),
                        //Channel = v.Channel
                    }).ToList()
                });
            }

            _logger.LogDebug($"Found {results.Count} results");

            return Ok(new ApiResponse<List<ManifestSearchResponseSchema>>(results));

            //var output = new JObject(new JProperty("Data", new JArray(output_data)));
            //_logger.LogDebug(output.ToString());
            //return new JsonResult(output);
        }
    }
}