using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WingetNexus.Shared.Helpers;

namespace WingetNexus.Shared.Models.Db
{
    public class Package
    {
        public Package()
        { }

        public Package(string identifier, string name, string publisher)
        {
            Identifier = identifier;
            Name = name;
            Publisher = publisher;
            Versions = new List<PackageVersion>();
        }


        public int Id { get; set; }
        public string Identifier { get; set; }
        public string Name { get; set; }
        public string Publisher { get; set; }
        public virtual ICollection<PackageVersion> Versions { get; set; }
        public int DownloadCount { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.Now;
        //public DateTime DateLastUpdated { get; set; }
        //public string Creator { get; set; }
        //public string LastUpdator { get; set; }


        public JObject GenerateOutput()
        {
            var versionData = new JArray();
            foreach (var version in Versions)
            {
                var data = new JObject(
                    new JProperty("PackageInstaller", version.VersionCode),
                    new JProperty("PackageLocale", GetDefaultLocale(version)),
                    new JProperty("Installers", GetInstallerData(version))
                );
                // Only append version if there's at least one installer
                if (data["Installers"].HasValues)
                {
                    versionData.Add(data);
                }
            }

            var output = new JObject(
                new JProperty("Data", new JObject(
                    new JProperty("Identifier", Identifier),
                    new JProperty("Versions", versionData)
                ))
            );
            return output;
        }

        private JObject GetDefaultLocale(PackageVersion version)
        {
            return new JObject(
                new JProperty("PackageLocale", version.DefaultLocale), //version.DefaultLocale.PackageLocale),
                new JProperty("Publisher", Publisher),
                new JProperty("PackageName", Name),
                new JProperty("ShortDescription", version.ShortDescription)
            );
        }

        private JArray GetInstallerData(PackageVersion version)
        {
            var installerData = new JArray();
            foreach (var installer in version.Installers)
            {
                if (installer.Scope == "both")
                {
                    // If installer is for both user and machine, create two entries for each scope (user and machine) but use it with download url
                    foreach (var scope in new[] { "user", "machine" })
                    {
                        var data = new JObject(
                            new JProperty("Architecture", installer.Architecture),
                            new JProperty("InstallerType", installer.InstallerType),
                            new JProperty("InstallerUrl", UrlHelpers.UrlFor("Download", "Api", Identifier, version.VersionCode, installer.Architecture, installer.Scope, "")),
                            new JProperty("InstallerSha256", installer.InstallerSha256),
                            new JProperty("Scope", scope),
                            new JProperty("InstallerSwitches", GetInstallerSwitches(installer))
                        );
                        if (installer.InstallerType == "zip")
                        {
                            data.Add(new JProperty("NestedInstallerType", installer.NestedInstallerType));
                            data.Add(new JProperty("NestedInstallerFiles", GetNestedInstallerData(installer)));
                        }
                        installerData.Add(data);
                    }
                }
                else
                {
                    var data = new JObject(
                        new JProperty("Architecture", installer.Architecture),
                        new JProperty("InstallerType", installer.InstallerType),
                        new JProperty("InstallerUrl", UrlHelpers.UrlFor("Download", "Api", Identifier, version.VersionCode, installer.Architecture, installer.Scope, "")),
                        new JProperty("InstallerSha256", installer.InstallerSha256),
                        new JProperty("Scope", installer.Scope),
                        new JProperty("InstallerSwitches", GetInstallerSwitches(installer))
                    );
                    if (installer.InstallerType == "zip")
                    {
                        data.Add(new JProperty("NestedInstallerType", installer.NestedInstallerType));
                        data.Add(new JProperty("NestedInstallerFiles", GetNestedInstallerData(installer)));
                    }
                    installerData.Add(data);
                }
            }
            return installerData;
        }

        private JObject GetInstallerSwitches(Installer installer)
        {
            var switches = new JObject();
            if (installer.Switches == null)
            {
                return switches;
            }

            foreach (var switchObj in installer.Switches)
            {
                switches.Add(switchObj.Parameter, switchObj.Value);
            }
            return switches;
        }

        private JArray GetNestedInstallerData(Installer installer)
        {
            var nestedInstallerData = new JArray();
            foreach (var nestedInstallerFile in installer.NestedInstallerFiles)
            {
                var data = new JObject(
                    new JProperty("RelativeFilePath", nestedInstallerFile.RelativeFilePath),
                    new JProperty("PortableCommandAlias", nestedInstallerFile.PortableCommandAlias)
                );
                nestedInstallerData.Add(data);
            }
            return nestedInstallerData;
        }

        public JObject GenerateOutputManifestSearch()
        {
            var versionData = new JArray();
            foreach (var version in Versions)
            {
                var data = new JObject(
                    new JProperty("PackageInstaller", version.VersionCode)
                );
                // Only append version if there's at least one installer
                if (version.Installers.Any())
                {
                    versionData.Add(data);
                }
            }

            var output = new JObject(
                new JProperty("Identifier", Identifier),
                new JProperty("PackageName", Name),
                new JProperty("Publisher", Publisher),
                new JProperty("Versions", versionData)
            );
            return output;
        }
    }
}
