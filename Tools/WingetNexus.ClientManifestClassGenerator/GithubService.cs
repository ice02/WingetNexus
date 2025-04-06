using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WingetNexus.ClientManifestClassGenerator
{
    public class GithubService
    {
        private readonly HttpClient client = new HttpClient();
        private readonly string owner = "microsoft";
        private readonly string repo = "winget-cli";
        private readonly string branch = "master";

        private readonly Dictionary<string, string> manifestPaths = new Dictionary<string, string>()
        {
            { "Installer", "https://raw.githubusercontent.com/{0}/{1}/refs/heads/{2}/schemas/JSON/manifests/v{3}/manifest.installer.{3}.json" },
            { "Version", "https://raw.githubusercontent.com/{0}/{1}/refs/heads/{2}/schemas/JSON/manifests/v{3}/manifest.version.{3}.json" },
            { "DefaultLocale", "https://raw.githubusercontent.com/{0}/{1}/refs/heads/{2}/schemas/JSON/manifests/v{3}/manifest.defaultLocale.{3}.json" },
            { "Locale", "https://raw.githubusercontent.com/{0}/{1}/refs/heads/{2}/schemas/JSON/manifests/v{3}/manifest.locale.{3}.json" },
            { "Singleton", "https://raw.githubusercontent.com/{0}/{1}/refs/heads/{2}/schemas/JSON/manifests/v{3}/manifest.singleton.{3}.json" }
        };

        public async Task<Dictionary<string, string>> GetManifestFilesForVersion(string version, string manifestType)
        {
            Console.WriteLine($"Getting manifest files for version {version} and type {manifestType}");

            var result = new Dictionary<string, string>();
            var manifests = manifestPaths;
            if (manifestType.ToUpper() != "ALL")
            {
                manifests = manifestPaths.Where(x => x.Key == manifestType).ToDictionary();
            }
            
            foreach (var path in manifests)
            {
                result.Add(path.Key, await GetFileFromRepo(path.Value, owner, repo, branch, version));
            }

            return result;
        }

        async Task<string> GetFileFromRepo(string path, string owner, string repo, string branch, string version)
        {
            Console.WriteLine($"Getting file from {path} for version {version}");

            string url = string.Format(path, owner, repo, branch, version);
            //client.DefaultRequestHeaders.UserAgent.ParseAdd("request");

            string content = string.Empty;
            try
            {
                content = await client.GetStringAsync(url);
                if (string.IsNullOrEmpty(content))
                {
                    throw new Exception("Error downloading file");  
                }
            }
            catch (Exception)
            {
                throw;
            }

            return content;
        }
    }
    public class ManifestPathGenerator
    {
        public Dictionary<string, string> GenerateManifestPaths(bool useDirectSchemaLink, string manifestVersion)
        {
            var manifestPaths = new Dictionary<string, string>
            {
                ["version"] = useDirectSchemaLink
                    ? $"https://raw.githubusercontent.com/microsoft/winget-cli/master/schemas/JSON/manifests/v{manifestVersion}/manifest.version.{manifestVersion}.json"
                    : $"https://aka.ms/winget-manifest.version.{manifestVersion}.schema.json",

                ["defaultLocale"] = useDirectSchemaLink
                    ? $"https://raw.githubusercontent.com/microsoft/winget-cli/master/schemas/JSON/manifests/v{manifestVersion}/manifest.defaultLocale.{manifestVersion}.json"
                    : $"https://aka.ms/winget-manifest.defaultLocale.{manifestVersion}.schema.json",

                ["locale"] = useDirectSchemaLink
                    ? $"https://raw.githubusercontent.com/microsoft/winget-cli/master/schemas/JSON/manifests/v{manifestVersion}/manifest.locale.{manifestVersion}.json"
                    : $"https://aka.ms/winget-manifest.locale.{manifestVersion}.schema.json",

                ["installer"] = useDirectSchemaLink
                    ? $"https://raw.githubusercontent.com/microsoft/winget-cli/master/schemas/JSON/manifests/v{manifestVersion}/manifest.installer.{manifestVersion}.json"
                    : $"https://aka.ms/winget-manifest.installer.{manifestVersion}.schema.json",

                ["singleton"] = useDirectSchemaLink
                    ? $"https://raw.githubusercontent.com/microsoft/winget-cli/master/schemas/JSON/manifests/v{manifestVersion}/manifest.singleton.{manifestVersion}.json"
                    : $"https://aka.ms/winget-manifest.singleton.{manifestVersion}.schema.json"
            };

            return manifestPaths;
        }
    }
}
