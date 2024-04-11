using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WingetNexus.Shared.Models.Dtos
{
    public class InstallerDto
    {
        public int Id { get; set; }
        public int PackageVersionId { get; set; }

        [Required]
        public string? Architecture { get; set; }
        [Required]
        public string? InstallerType { get; set; }
        [Required]
        public string? InstallerPath { get; set; }
        [Required]
        public bool IsLocalPackage { get; set; } = true;
        [Required]
        public string? InstallerSha256 { get; set; }
        [Required]
        public string? Scope { get; set; }

        public List<VersionDto> PackagesDependencies { get; set; }
        public List<string> ExternalDependencies { get; set; }
        public List<WindowsFeatureDependencyDto> WindowsFeatures { get; set; }
        public List<WindowsComponentDependencyDto> WindowsLibraries { get; set; }

        public string? NestedInstallerType { get; set; }

        //public string? FullUrl { get; set; }

        //public SwitchesDto Switches { get; set; } = new SwitchesDto();
        public List<SwitchDto> Switches { get; set; } = new List<SwitchDto>();
        public List<NestedInstallerFileDto>? NestedInstallerFiles { get; set; } = new List<NestedInstallerFileDto>();
        
        public DateTime DateAdded { get; set; } = DateTime.Now;
        public DateTime DateModified { get; set; } = DateTime.Now;

        
    }
}
