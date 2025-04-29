using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WingetNexus.Shared.Models.Dtos
{
    public class ApplicationDto
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public PublisherDto Publisher { get; set; }
        [Required]
        public string PackageIdentifier { get; set; }

        public string? GitHubUrl { get; set; }

        public List<VersionDto> Versions { get; set; } = new List<VersionDto>();

        public VersionDto? LatestVersion = null;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        public string UserCreated { get; set; } = string.Empty;
        public string UserLastModified { get; set; } = string.Empty;

        public ApplicationDto(string name, PublisherDto publisher, string identifier, string architecture, string version, string installerType)
        {
            Name = name;
            Publisher = publisher;
            PackageIdentifier = identifier;

            //Versions.Add(new VersionDto()
            //{ 
            //    VersionNumber = version,
            //    Installers = new List<InstallerDto>()
            //    {
            //        new InstallerDto()
            //        {
            //            Architecture = architecture,
            //            InstallerType = installerType
            //        }
            //    }
            //});

            LatestVersion = Versions.OrderByDescending(p => p.CreatedDate).FirstOrDefault();
        }

        public ApplicationDto()
        {

        }

        public ApplicationDto(
            string identifier, 
            string name, 
            PublisherDto publisher)
        {
            PackageIdentifier = identifier;
            Name = name;
            Publisher = publisher;
        }
    }
}
