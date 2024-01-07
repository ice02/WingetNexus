using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WingetNexus.Shared.Models.Winget
{
    public class ManifestExtended
    {
        public string PackageIdentifier { get; set; }
        public ManifestVersion[] Versions { get; set; }
    }
}
