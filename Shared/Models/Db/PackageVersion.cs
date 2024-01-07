using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WingetNexus.Shared.Models.Db
{
    public class PackageVersion
    {
        public PackageVersion()
        {

        }

        //contructor for package version with identifier, version code, default locale, package locale, and short description
        //version, "en-US", name, identifier
        public PackageVersion(string versionCode, string defaultLocale, string identifier, string shortdescription)
        {
            Identifier = identifier;
            VersionCode = versionCode;
            //DefaultLocale = defaultLocale; //new Locale(defaultLocale);
            PackageLocale = defaultLocale;
            ShortDescription = shortdescription;

        }

        public int Id { get; set; }
        public string Identifier { get; set; }
        public string VersionCode { get; set; }
        public virtual Locale DefaultLocale { get; set; }
        public string? Channel { get; set; }
        public string ShortDescription { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.Now;
        public virtual ICollection<Installer> Installers { get; set; }
        //public virtual ICollection<Locale> Locales { get; set; }
        //public virtual ICollection<String> Locales { get; set; }
        [JsonIgnore]
        public virtual Package Package { get; set; }
        [ForeignKey("Package")]
        public int PackageId { get; set; }
        [ForeignKey("DefaultLocale")]
        public string PackageLocale { get; set; }

        //public DateTime DateLastUpdated { get; set; }
        //public string Creator { get; set; }
        //public string LastUpdator { get; set; }
    }
}
