using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WingetNexus.Shared.Models.Winget
{
    public class ManifestLocal
    {
        public string PackageLocale { get; set; }
        public string? Publisher { get; set; }
        public string? PublisherUrl { get; set; }
        public string? PublisherSupportUrl { get; set; }
        public string? PrivacyUrl { get; set; }
        public string? Author { get; set; }
        public string? PackageName { get; set; }
        public string? PackageUrl { get; set; }
        public string? License { get; set; }
        public string? LicenseUrl { get; set; }
        public string? Copyright { get; set; }
        public string? CopyrightUrl { get; set; }
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public string? Tags { get; set; }
        public string? ReleaseNotes { get; set; }
        public string? ReleaseNotesUrl { get; set; }
    }
}
