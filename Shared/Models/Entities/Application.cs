using System;
using System.Collections.Generic;

namespace WingetNexus.Shared.Entities
{
    public class Application
    {
        public Application(string identifier, string name, Publisher publisher)
        {
            Identifier = identifier;
            Name = name;
            Publisher = publisher;
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public string? PackageIdentifier { get; set; }
        public string? GitHubUrl { get; set; }

        public int PublisherId { get; set; }
        public Publisher? Publisher { get; set; }

        public ICollection<Version>? Versions { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;
        public string Identifier { get; }
    }
}
