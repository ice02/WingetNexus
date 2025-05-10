using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WingetNexus.Shared.Models.Winget
{
    public class PackageManifest
    {
        /// <summary>
        /// Gets or sets PackageIdentifier.
        /// </summary>
        public string PackageIdentifier { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets PackageName.
        /// </summary>
        public string PackageName { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets Publisher.
        /// </summary>
        public string Publisher { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets SearchVersions.
        /// </summary>
        public ManifestVersion[] Versions { get; set; } = Array.Empty<ManifestVersion>();
    }
}
