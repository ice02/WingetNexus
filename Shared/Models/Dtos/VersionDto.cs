using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WingetNexus.Shared.Models.Yaml;

namespace WingetNexus.Shared.Models.Dtos
{
    public class VersionDto
    {
        public int Id { get; set; }
        public string VersionNumber { get; set; } = string.Empty;
        public string? GitHubUrl { get; set; }
        public List<IInstallerClass>? Installers { get; set; }
        public List<ILocaleClass>? Locales { get; set; }
        public IDefaultLocaleClass? DefaultLocale { get; set; }
        public string InstallersDatasJson { get; set; } = string.Empty;
        public string LocalsDatasJson { get; set; } = string.Empty;
        public string DefaultLocaleKey { get; set; } = string.Empty;
        public string? ManifestVersion { get; set; } = string.Empty;

        public string ShortDescription { get; set; }
        //public required string? PackageLocale { get; set; }
        
        public string Channel { get; set; } = "Stable";

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        public string UserCreated { get; set; } = string.Empty;
        public string UserLastModified { get; set; } = string.Empty;

        public ApplicationDto? Application { get; set; }
        public string ApplicationIdentifier { get; set; } = string.Empty;
    }
}
