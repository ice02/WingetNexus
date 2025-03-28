using System.ComponentModel.DataAnnotations.Schema;

namespace WingetNexus.Shared.Entities
{
    public class Version
    {
        public int Id { get; set; }
        public required string VersionNumber { get; set; }
        public string? GitHubUrl { get; set; }
        [NotMapped]
        public object? Datas { get; set; }
        public required string DatasJson { get; set; }
        public required string ManifestVersion { get; set; }
        public required string DefaultLocale { get; set; }

        public required int ApplicationId { get; set; }
        public required Application Application { get; set; }
        public required ICollection<Locale> Locales { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;
    }
}
