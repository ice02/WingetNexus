using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WingetNexus.Shared.Helpers;
using WingetNexus.Shared.Models.Db;
using WingetNexus.Shared.Models.Db.Objects;
using WingetNexus.Shared.Models.Dtos;

namespace WingetNexus.Data.DataStores
{
    public class WingetNexusDataStore : IWingetNexusDataStore
    {
        private readonly ILogger<WingetNexusDataStore> _logger;
        private readonly IConfiguration _configuration;

        public WingetNexusDataStore(ILogger<WingetNexusDataStore> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        //create installer in context from installer dto
        public Installer CreateInstaller(InstallerDto installerForm)
        {
            var installer = new Installer(installerForm.Architecture, installerForm.InstallerType, installerForm.InstallerPath,
                                                installerForm.InstallerSha256, installerForm.Scope);

            installer.Switches = new List<InstallerSwitch>()
                    {
                        new InstallerSwitch() { Parameter="Silent", Value="True" },
                        new InstallerSwitch() { Parameter="InstallLocation", Value="" },
                        new InstallerSwitch() { Parameter="SilentProgress", Value="True" },
                        new InstallerSwitch() { Parameter="Interactive", Value="False" },
                        new InstallerSwitch() { Parameter="Upgrade", Value="False" },
                        new InstallerSwitch() { Parameter="Logfile", Value="" },
                        new InstallerSwitch() { Parameter="Custom", Value="" }
                    };

            if (installerForm.InstallerType == "zip")
            {
                installer.Switches.Add(new InstallerSwitch() { Parameter = "ExtractPath", Value = "" });
                installer.NestedInstallerFiles = new List<NestedInstallerFile>()
                        {
                            new NestedInstallerFile()
                            {
                                RelativeFilePath = installerForm.NestedInstallerFiles[0].RelativeFilePath,
                                PortableCommandAlias = installerForm.NestedInstallerFiles[0].PortableCommandAlias//FileHelper.ExtractNameFromCommandLine(installerForm.NestedInstallerFiles[0].RelativeFilePath)
                            }
                        };
            }

            return installer;
        }

        public PackageVersion CreateVersion(VersionDto versionForm)
        {
            var version = new PackageVersion(versionForm.VersionCode, versionForm.PackageLocale, versionForm.PackageLocale, versionForm.ShortDescription);

            foreach (var installerForm in versionForm.Installers)
            {
                var installer = CreateInstaller(installerForm);
                version.Installers.Add(installer);
            }

            return version;
        }





    }
}
