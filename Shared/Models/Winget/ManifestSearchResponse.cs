using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WingetNexus.Shared.Models.Winget
{
    public class ManifestSearchResponse
    { 
        /// <summary>
        ///        /// Gets or sets Manifests.
        ///               /// </summary>
        public List<ManifestSearchResponseItem> Data { get; set; }
    }

    public class ManifestSearchResponseItem
    {
        /// <summary>
        /// Gets or sets PackageIdentifier.
        /// </summary>
        public string PackageIdentifier { get; set; }

        /// <summary>
        /// Gets or sets PackageName.
        /// </summary>
        public string PackageName { get; set; }

        /// <summary>
        /// Gets or sets Publisher.
        /// </summary>
        public string Publisher { get; set; }

        /// <summary>
        /// Gets or sets SearchVersions.
        /// </summary>
        public SearchVersions[] Versions { get; set; }
    }
}
