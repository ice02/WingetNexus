using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WingetNexus.Shared.Models.Winget
{
    public class ManifestInstaller
    {
        public string Architecture { get; set; }
        public string InstallerType { get; set; }
        public string InstallerUrl { get; set; }
        public string InstallerSha256 { get; set; }
        public string Scope { get; set; }
        public dynamic[] InstallerSwitches { get; set; }
        public string? NestedInstallerType { get; set; }
        public ManifestNestedInstallerFile[]? NestedInstallerFiles { get; set; }
    }
}
