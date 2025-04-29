using System.ComponentModel.DataAnnotations.Schema;
using WingetNexus.Shared.Models.Yaml;

namespace WingetNexus.Shared.Models.Dtos
{
    public class VersionDto
    {
        public int Id { get; set; }
        public string? VersionNumber { get; set; }
        public string? GitHubUrl { get; set; }
        public string? DefaultLocaleValue { get; set; } = string.Empty;

        [NotMapped]
        public List<IInstallerClass>? Installers { get; set; }
        [NotMapped]
        public List<ILocaleClass>? Locales { get; set; }
        [NotMapped]
        public IDefaultLocaleClass? DefaultLocale { get; set; }
        [NotMapped]
        public IVersionClass? VersionData { get; set; }

        public int VersionDatasKey { get; set; }
        public int InstallersDatasKey { get; set; }
        public List<int>? LocalsDatasKey { get; set; }
        public int DefaultLocaleKey { get; set; }

        public ContentFileDto? VersionContent { get; set; }
        public ContentFileDto? InstallersContent { get; set; }
        public List<ContentFileDto>? LocalesContent { get; set; }
        public ContentFileDto? DefaultLocaleContent { get; set; }

        public string ManifestVersion { get; set; } = string.Empty;

        public string ShortDescription { get; set; } = string.Empty;

        public string Channel { get; set; } = string.Empty;

        public int ApplicationId { get; set; }
        public ApplicationDto? Application { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        public string UserCreated { get; set; } = string.Empty;
        public string UserLastModified { get; set; } = string.Empty;

        
        public string ApplicationIdentifier { get; set; } = string.Empty;
    }
}
