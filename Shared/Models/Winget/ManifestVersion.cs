using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WingetNexus.Shared.Models.Winget
{
    public class ManifestVersion
    {
        public string PackageVersion { get; set; }
        public string Channel { get; set; }
        public ManifestLocal DefaultLocale { get; set; }
        public ManifestLocal[] Locales { get; set; }
        public ManifestInstaller[] Installers { get; set; }
    }
}
