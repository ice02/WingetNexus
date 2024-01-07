using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WingetNexus.Shared.Models.Db.Objects
{
    public class Dependencies
    {
        /// <summary>
        /// Gets or sets WindowsFeatures.
        /// </summary>
        public string[] WindowsFeatures { get; set; }

        /// <summary>
        /// Gets or sets WindowsLibraries.
        /// </summary>
        public string[] WindowsLibraries { get; set; }

        /// <summary>
        /// Gets or sets PackageDependencies.
        /// </summary>
        public string[] PackageDependencies { get; set; }

        /// <summary>
        /// Gets or sets ExternalDependencies.
        /// </summary>
        public string[] ExternalDependencies { get; set; }
    }
}
