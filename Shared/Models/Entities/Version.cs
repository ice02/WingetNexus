using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using WingetNexus.Shared.Models.Yaml;

namespace WingetNexus.Shared.Models.Entities
{
    public class Version
    {
        public int Id { get; set; }
        public string VersionNumber { get; set; } = string.Empty;
        public string? GitHubUrl { get; set; }
        public string DefaultLocaleValue { get; set; } = string.Empty;

        [NotMapped]
        public List<IInstallerClass>? Installers { get; set; }
        [NotMapped]
        public List<ILocaleClass>? Locales { get; set; }
        [NotMapped]
        public IDefaultLocaleClass? DefaultLocale { get; set; }
        [NotMapped]
        public IVersionClass? VersionData { get; set; }

        public int VersionContentFK { get; set; }
        public int InstallersContentFK { get; set; }
        public int DefaultLocaleFK { get; set; }

        public required string ManifestVersion { get; set; } = string.Empty;

        public virtual ContentFiles? VersionContent { get; set; }
        public virtual ContentFiles? InstallersContent { get; set; }
        public virtual IEnumerable<ContentFiles>? LocalesContent { get; set; }
        public virtual ContentFiles? DefaultLocaleContent { get; set; }

        public required string ShortDescription { get; set; } = string.Empty;

        public string Channel { get; set; } = string.Empty;

        public int ApplicationId { get; set; }
        public virtual required Application Application { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        public string UserCreated { get; set; } = string.Empty;
        public string UserLastModified { get; set; } = string.Empty;
    }
}
