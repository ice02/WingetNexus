using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WingetNexus.Shared.Models.Yaml;

namespace WingetNexus.Shared.Mappers
{
    public class LocaleMapper
    {
        public IDefaultLocaleClass MapToDefaultLocale(ILocaleClass locale)
        {
            return new DefaultLocaleClass
            {
                PackageIdentifier = locale.PackageIdentifier,
                PackageVersion = locale.PackageVersion,
                PackageLocale = locale.PackageLocale,
                Publisher = locale.Publisher,
                ManifestVersion = locale.ManifestVersion
            };
        }
    }

    public class DefaultLocaleClass : IDefaultLocaleClass
    {
        public string PackageIdentifier { get; set; }
        public string PackageVersion { get; set; }
        public string PackageLocale { get; set; }
        public string Publisher { get; set; }
        public string ManifestVersion { get; set; }
    }
}
