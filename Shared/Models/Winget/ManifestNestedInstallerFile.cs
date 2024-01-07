using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WingetNexus.Shared.Models.Winget
{
    public class ManifestNestedInstallerFile
    {
        /// <summary>
        /// Gets or sets RelativeFilePath.
        /// </summary>
        public string RelativeFilePath { get; set; }

        /// <summary>
        /// Gets or sets PortableCommandAlias.
        /// </summary>
        public string PortableCommandAlias { get; set; }
    }
}
