using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WingetNexus.Shared.Models.Db
{
    public class Locale
    {
        public Locale()
        { }

        public Locale(string localCode)
        {
            this.PackageLocale = localCode;
        }

        /// <summary>
        /// Gets or sets PackageLocale.
        /// </summary>
        [Key]
        public string PackageLocale { get; set; }

        /// <summary>
        /// Gets or sets Publisher.
        /// </summary>
        public string? Publisher { get; set; }

        /// <summary>
        /// Gets or sets PublisherUrl.
        /// </summary>
        public string? PublisherUrl { get; set; }

        /// <summary>
        /// Gets or sets PublisherSupportUrl.
        /// </summary>
        public string? PublisherSupportUrl { get; set; }

        /// <summary>
        /// Gets or sets PrivacyUrl.
        /// </summary>
        public string? PrivacyUrl { get; set; }

        /// <summary>
        /// Gets or sets Author.
        /// </summary>
        public string? Author { get; set; }

        /// <summary>
        /// Gets or sets PackageName.
        /// </summary>
        public string? PackageName { get; set; }

        /// <summary>
        /// Gets or sets PackageUrl.
        /// </summary>
        public string? PackageUrl { get; set; }

        /// <summary>
        /// Gets or sets License.
        /// </summary>
        public string? License { get; set; }

        /// <summary>
        /// Gets or sets LicenseUrl.
        /// </summary>
        public string? LicenseUrl { get; set; }

        /// <summary>
        /// Gets or sets Copyright.
        /// </summary>
        public string? Copyright { get; set; }

        /// <summary>
        /// Gets or sets CopyrightUrl.
        /// </summary>
        public string? CopyrightUrl { get; set; }

        /// <summary>
        /// Gets or sets ShortDescription.
        /// </summary>
        public string? ShortDescription { get; set; }

        /// <summary>
        /// Gets or sets Description.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets ReleaseNotes.
        /// </summary>
        public string? ReleaseNotes { get; set; }

        /// <summary>
        /// Gets or sets ReleaseNotesUrl.
        /// </summary>
        public string? ReleaseNotesUrl { get; set; }

        /// <summary>
        /// Gets or sets Agreements.
        /// </summary>
        //public Agreements Agreements { get; set; }

        /// <summary>
        /// Gets or sets Tags.
        /// </summary>
        //public Tags Tags { get; set; }

        /// <summary>
        /// Gets or sets purchaseUrl.
        /// </summary>
        public string? PurchaseUrl { get; set; }

        /// <summary>
        /// Gets or sets installationNotes.
        /// </summary>
        //public InstallationNotes InstallationNotes { get; set; }

        /// <summary>
        /// Gets or sets documentations.
        /// </summary>
        //public Documentations Documentations { get; set; }

        /// <summary>
        /// Gets or sets icons.
        /// </summary>
        //public Icons Icons { get; set; }
    }
}
