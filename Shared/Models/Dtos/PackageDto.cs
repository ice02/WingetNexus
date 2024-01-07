using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WingetNexus.Shared.Models.Dtos
{
    public class PackageDto
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Publisher { get; set; }
        [Required]
        public string Identifier { get; set; }
        
        public List<VersionDto> Versions { get; set; } = new List<VersionDto>();

        public DateTime DateAdded { get; set; } = DateTime.Now;

        public PackageDto(string name, string publisher, string identifier, string architecture, string version, string installerType)
        {
            Name = name;
            Publisher = publisher;
            Identifier = identifier;

            Versions.Add(new VersionDto()
            { 
                VersionCode = version,
                Installers = new List<InstallerDto>()
                {
                    new InstallerDto()
                    {
                        Architecture = architecture,
                        InstallerType = installerType
                    }
                }
            });
        }

        public PackageDto()
        {

        }
    }
}
