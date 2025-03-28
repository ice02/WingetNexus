namespace WingetNexus.Shared.Models.Github
{
    public class ApplicationInfo
    {
        public required string FilePath { get; set; }
        public required string Publisher { get; set; }
        public required string Name { get; set; }
        public required string Version { get; set; }

        public List<string>? YamlFilesList { get; set; }
    }
}
