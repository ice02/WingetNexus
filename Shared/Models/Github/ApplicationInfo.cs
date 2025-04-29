using WingetNexus.Shared.Models.Dtos;

namespace WingetNexus.Shared.Models.Github
{
    public class ApplicationInfo
    {
        public required string FilePath { get; set; }
        public required string Publisher { get; set; }
        public required string PublisherUrl { get; set; }
        public required string Name { get; set; }
        public required string ApplicationUrl { get; set; }
        public required string Version { get; set; }
        public required string VersionUrl { get; set; }

        public List<ContentFileDto>? YamlFilesList { get; set; }
    }
}
