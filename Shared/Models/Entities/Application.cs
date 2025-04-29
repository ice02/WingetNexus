using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WingetNexus.Shared.Models.Entities
{
    public class Application
    {
        public Application()
        {
        }

        public Application(string identifier, string name, Publisher publisher)
        {
            PackageIdentifier = identifier;
            Name = name;
            Publisher = publisher;
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public string? PackageIdentifier { get; set; }
        public string? GitHubUrl { get; set; }

        public int PublisherId { get; set; }
        public virtual Publisher? Publisher { get; set; }

        public virtual ICollection<Version>? Versions { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        public string UserCreated { get; set; } = string.Empty;
        public string UserLastModified { get; set; } = string.Empty;
    }
}
