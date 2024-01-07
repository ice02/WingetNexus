using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WingetNexus.Shared.Models.Dtos
{
    public class VersionDto
    {
        public int Id { get; set; }
        public string Identifier { get; set; }
        [Required]
        public string VersionCode { get; set; }
        [Required]
        public string ShortDescription { get; set; }
        [Required]
        public string? PackageLocale { get; set; }
        public int PackageId { get; set; }
        public string Channel { get; set; } = "Stable";

        public DateTime DateAdded { get; set; } = DateTime.Now;
        [Required]
        public List<InstallerDto> Installers { get; set; } = new List<InstallerDto>();
    }
}
