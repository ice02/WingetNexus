using System.Collections.Generic;

namespace WingetNexus.Shared.Models.Entities
{
    public class Publisher
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? GitHubUrl { get; set; }

        public ICollection<Application>? Applications { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;
    }
}
