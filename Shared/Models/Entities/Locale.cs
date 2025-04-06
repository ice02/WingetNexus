

namespace WingetNexus.Shared.Models.Entities
{
    public class Locale
    {
        public int Id { get; set; }
        public required string PackageLocale { get; set; }
        public required string JsonContent { get; set; }

        public required string JsonVersion { get; set; }

        public int VersionId { get; set; }
        public required Version Version { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;
    }
}
