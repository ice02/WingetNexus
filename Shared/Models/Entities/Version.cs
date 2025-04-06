using System.ComponentModel.DataAnnotations.Schema;
using WingetNexus.Shared.Models.Yaml;

namespace WingetNexus.Shared.Models.Entities
{
    public class Version
    {
        public int Id { get; set; }
        public required string VersionNumber { get; set; }
        public string? GitHubUrl { get; set; }
        [NotMapped]
        public List<IInstallerClass>? Installers { get; set; }
        [NotMapped]
        public List<ILocaleClass>? Locales { get; set; }
        [NotMapped]
        public IDefaultLocaleClass? DefaultLocale { get; set; }
        public required string InstallersDatasJson { get; set; } = string.Empty;
        public required string LocalsDatasJson { get; set; } = string.Empty;
        public required string ManifestVersion { get; set; } = string.Empty;
        public required string DefaultLocaleKey { get; set; } = string.Empty;
        public required string DefaultLocaleJson { get; set; } = string.Empty;

        public required string ShortDescription { get; set; } = string.Empty;

        public string Channel { get; set; } = string.Empty;

        public int ApplicationId { get; set; }
        public required Application Application { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        public string UserCreated { get; set; } = string.Empty;
        public string UserLastModified { get; set; } = string.Empty;
    }
}
