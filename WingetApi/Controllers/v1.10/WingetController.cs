using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.FeatureManagement;
using Newtonsoft.Json.Linq;
using Serilog;
using WingetNexus.Data;
using WingetNexus.Data.DataStores;
using WingetNexus.Shared.Helpers;
using WingetNexus.Shared.Models.Winget;
using WingetNexus.Shared.Models.Winget._1._7;
using WingetNexus.Shared.Models.Yaml;
using WingetNexus.Shared.Utils;
using WingetNexus.WingetApi.Data;
using WingetNexus.WingetApi.Helpers;
using WingetNexus.WingetApi.Services;

namespace WingetNexus.WingetApi.Controllers.v1._10
{
    /// <summary>
    /// Api for winget client based on winget contract version 1.10
    /// </summary>
    [Route("api/v1.10/[controller]")]
    [ApiController]
    [IgnoreAntiforgeryToken]
    public class WingetController : ControllerBase
    {
        //contructor with dependency injection ilogger and datacontext
        private readonly WingetNexusContext _context;
        private readonly ILogger<WingetController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IFeatureManager _featureManager;
        private readonly ILocalCacheService _localCacheService;
        private readonly IWingetApiDatastore _dataStore;

        public WingetController(ILogger<WingetController> logger, WingetNexusContext context, IConfiguration configuration, 
            IFeatureManager featureManager, ILocalCacheService localCacheService, IWingetApiDatastore wingetApiDatastore)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
            _featureManager = featureManager;
            _localCacheService = localCacheService;
            _dataStore = wingetApiDatastore;
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
        [HttpGet]
        public async Task<IActionResult> GetCache()
        {
            try
            {
                var filepath = await _localCacheService.GetSource2MsixAsync();
                if (string.IsNullOrEmpty(filepath))
                {
                    return NotFound("File not found in cache.");
                }

                var fileContent = System.IO.File.ReadAllBytes(filepath);

                if (fileContent == null)
                {
                    return NotFound("File not found in cache.");
                }

                return File(fileContent, "application/octet-stream", "cacheFile");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cache file.");
                return StatusCode(500, "Internal server error.");
            }
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

            var info = new InformationResponseSchema()
            {
                Data = new InformationSchema()
                {
                    SourceIdentifier = _configuration["Reponame"],
                    ServerSupportedVersions = new ServerSupportedVersions() { "1.0.0", "1.1.0", "1.4.0", "1.7.0", "1.10.0" },
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
        }

        [HttpPost("manifestSearch")]
        public async Task<IActionResult> ManifestSearch([FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] ManifestSearchRequestSchema request_data)
        {
            if (_featureManager.IsEnabledAsync("RequireAuthentication").Result)
            {
                CertificateValidationHelper.ValidateAuthentication(Request, _logger);
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
            var packages = new List<Shared.Models.Entities.Application>();
            if (keyword != null && match_type != null)
            {
                var ctx_request = _context.Applications
                    .Include(p => p.Publisher)
                    .Include(p => p.Versions)
                    .Include("Versions.Installers")
                    .Include("Versions.Installers.Switches")
                    .Include("Versions.Installers.NestedInstallerFiles");

                if (match_type == "Exact")
                {
                    var packages_query = ctx_request.Where(p => p.PackageIdentifier == keyword);
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
                        packages_query = ctx_request.Where(p => p.PackageIdentifier.Contains(keyword));
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
                    PackageIdentifier = package.PackageIdentifier,
                    PackageName = package.Name,
                    Publisher = package.Publisher.Name,
                    Versions = package.Versions.Where(p => p.Installers.Any()).Select(v => new ManifestSearchVersionSchema()
                    {
                        PackageVersion = v.VersionNumber,
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

        [HttpGet("packageManifests/{packageIdentifier}")]
        public async Task<IActionResult> GetPackageManifest(string packageIdentifier)
        {
            //if (_featureManager.IsEnabledAsync("RequireAuthentication").Result)
            //{
            //    CertificateValidationHelper.ValidateAuthentication(Request, _logger);
            //}

            //// add to log all information from header
            //foreach (var header in Request.Headers)
            //{
            //    _logger.LogDebug($"{header.Key}: {header.Value}");
            //}

            ////get version from query string
            //var queryVersion = Request.Query["Version"].ToString();

            //var package = _context.Applications
            //                .Include(p => p.Versions)
            //                .Include(p => p.Publisher)
            //                .Include("Versions.DefaultLocale")
            //                .Include("Versions.Installers")
            //                .Include("Versions.Installers.NestedInstallerFiles")
            //                .Include("Versions.Installers.Switches")
            //                .FirstOrDefault(p => p.PackageIdentifier == identifier);

            //if (package == null)
            //{
            //    return NoContent();
            //}


            //var versionData = new List<IVersionClass>();
            //IVersionClass? data = null;
            //foreach (var version in package.Versions.Where(p=>p.VersionNumber == queryVersion))
            //{
            //    var installers = GetInstallerData(version);
            //    if (installers == null)
            //    {
            //        _logger.LogDebug($"No installer found for version {version.VersionNumber}");
            //        continue;
            //    }

            //    switch (version.ManifestVersion)
            //    {
            //        case "1.4.0": 
            //            data = new WingetNexus.Shared.Models.Yaml.v1._4.Version.VersionClass()
            //            {
            //                PackageIdentifier = version.Application.PackageIdentifier,
            //                PackageVersion = version.VersionNumber,
            //                DefaultLocale = version.DefaultLocaleValue,
            //                ManifestType = "version",
            //                ManifestVersion = version.ManifestVersion,
            //                Installers = installers.Result,
            //                Channel = version.Channel
            //            };
            //            break;
            //        case "1.5.0":
            //            data = new WingetNexus.Shared.Models.Yaml.v1._5.Version.VersionClass()
            //            {
            //                PackageIdentifier = version.Application.PackageIdentifier,
            //                PackageVersion = version.VersionNumber,
            //                DefaultLocale = version.DefaultLocaleValue,
            //                ManifestType = "version",
            //                ManifestVersion = version.ManifestVersion,
            //                Installers = installers.Result,
            //                Channel = version.Channel
            //            };
            //            break;
            //        case "1.6.0":
            //            break;
            //        case "1.7.0":
            //            break;
            //        case "1.9.0":
            //            break;
            //        case "1.10.0":
            //        default:
            //            break;
            //    }

            //    data = new Versions()
            //    {
            //        PackageVersion = version.VersionNumber,
            //        DefaultLocale = new DefaultLocale()
            //        {
            //            Moniker = version.Application.PackageIdentifier,
            //            PackageLocale = version.DefaultLocaleValue,
            //            Publisher = package.Publisher.Name,
            //            PackageName = package.Name,
            //            ShortDescription = version.ShortDescription
            //        },
            //        Channel = version.Channel, //version.Channel, 
            //        Installers = 
            //        //Locales = new ManifestLocal[0]
            //    };

            //    // Only append version if there's at least one installer
            //    //if (data.Installers.Any())
            //    //{
            //    //    versionData.Add(data);
            //    //}

            ApiDataPage<PackageManifest> manifests;
            QueryParameters unsupportedQueryParameters;
            QueryParameters requiredQueryParameters;
            Dictionary<string, string> headers = null;

            try
            {
                // Parse Headers
                headers = HeaderProcessor.ToDictionary(Request.Headers);
                string continuationToken = headers.GetValueOrDefault(QueryConstants.ContinuationToken);

                string versionFilter = null;
                string channelFilter = null;
                string marketFilter = null;

                // Schema supports query parameters only when PackageIdentifier is specified.
                if (!string.IsNullOrWhiteSpace(packageIdentifier))
                {
                    versionFilter = Request.Query[QueryConstants.Version];
                    channelFilter = Request.Query[QueryConstants.Channel];
                    marketFilter = Request.Query[QueryConstants.Market];
                }

                manifests = await this._dataStore.GetPackageManifests(packageIdentifier, continuationToken, versionFilter, channelFilter, marketFilter);
                unsupportedQueryParameters = UnsupportedAndRequiredFieldsHelper.GetUnsupportedQueryParametersFromRequest(req.Query, ApiConstants.UnsupportedQueryParameters);
                requiredQueryParameters = UnsupportedAndRequiredFieldsHelper.GetRequiredQueryParametersFromRequest(req.Query, ApiConstants.RequiredQueryParameters);
            }
            //catch (DefaultException e)
            //{
            //    log.LogError(e.ToString());
            //    return ActionResultHelper.ProcessError(e.InternalRestError);
            //}
            catch (Exception e)
            {
                _logger.LogError(e.ToString());

                //TODO: add metrics
                //if (await this.appConfig.IsEnabledAsync(FeatureFlag.GenevaLogging, null))
                //{
                //    Geneva.Metrics.EmitMetricForOperation(
                //        Geneva.ErrorMetrics.DatabaseUpdateError,
                //        FunctionConstants.ManifestGet,
                //        req.Path.Value,
                //    headers,
                //        e,
                //        log);
                //}

                return ActionResultHelper.UnhandledError(e);
            }

            return manifests.Items.Count switch
            {
                0 => new NoContentResult(),
                1 => new ApiObjectResult(new GetPackageManifestApiResponse<PackageManifest>(manifests.Items.First(), manifests.ContinuationToken)
                {
                    UnsupportedQueryParameters = unsupportedQueryParameters,
                    RequiredQueryParameters = requiredQueryParameters,
                }),
                _ => new ApiObjectResult(new GetPackageManifestApiResponse<List<PackageManifest>>(manifests.Items.ToList(), manifests.ContinuationToken)
                {
                    UnsupportedQueryParameters = unsupportedQueryParameters,
                    RequiredQueryParameters = requiredQueryParameters,
                }),
            };

            //return Ok(new ApiResponse<IVersionClass>(data));
            }

            //var output0 = package.GenerateOutput();
            //var output = new { Data = new { PackageIdentifier = package.Identifier, Versions = versionData } };
            ManifestSingleResponseSchema output = new ManifestSingleResponseSchema()
            {
                Data = new ManifestSchema()
                {
                    PackageIdentifier = package.PackageIdentifier,
                    Versions = versionData
                }
            };

            Response.Headers.Add("Version", _configuration["CurrentVersion"].ToString());

            return Ok(output);
        }

        private async Task<IInstallerClass?> GetInstallerData(Shared.Models.Entities.Version version)
        {
            // if (version.Installers == null)
            // {
            //     return new List<Installer>();
            // }

            // get yaml content file from version for installers
            var yamlContent = version.InstallersContent.FileContent;
            var yamlContentVersion = version.InstallersContent.FileVersion;
            var yamlContentType = version.InstallersContent.FileType;
            if (string.IsNullOrEmpty(yamlContent) || string.IsNullOrEmpty(yamlContentVersion))
            {
                return null;
            }

            // deserialize yaml content to real class based on version
            var yamlHelpers = new YamlHelpers();
            var deserializedYaml = await yamlHelpers.DeserializeYamlContent(yamlContentType, yamlContentVersion, yamlContent);
            if (deserializedYaml == null)
            {
                return null;
            }

            //var installerData = new List<Installer>();
            //foreach (var installer in deserializedYaml.Installers)
            //{

            //    if (installer.Scope == "both")
            //    {
            //        // If installer is for both user and machine, create two entries for each scope (user and machine) but use it with download url
            //        foreach (var scope in new[] { "user", "machine" })
            //        {
            //            AddManifestToInstaller(installerData, installer, scope);
            //        }
            //    }
            //    else
            //    {
            //        AddManifestToInstaller(installerData, installer, installer.Scope);
            //    }

            //}

            return deserializedYaml as IInstallerClass;
        }

        private async Task<Installer>

        private void AddManifestToInstaller(List<Installer> installerData, Shared.Models.Entities.Installer installer, string scope)
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

        private NestedInstallerFiles GetNestedInstallerData(Shared.Models.Entities.Installer installer)
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

        private InstallerSwitches GetInstallerSwitches(Shared.Models.Entities.Installer installer)
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


    }
}