using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WingetNexus.Shared.Models.Yaml
{
    public interface IDefaultLocaleClass
    {
        public string PackageIdentifier { get; set; }
        public string PackageVersion { get; set; }
        public string PackageLocale { get; set; }
        public string Publisher { get; set; }
        public string ManifestVersion { get; set; }
    }
}
