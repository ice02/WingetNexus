using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WingetNexus.Shared.Models.Db.Objects
{
    public class PackageDependency
    {
        /// <summary>
        /// Gets or sets PackageIdentifier.
        /// </summary>
        public string? PackageIdentifier { get; set; }

        /// <summary>
        /// Gets or sets MinimumVersion.
        /// </summary>
        public string? MinimumVersion { get; set; }
    }
}
