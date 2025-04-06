using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WingetNexus.Shared.Models.Dtos;
using WingetNexus.Shared.Models.Entities;

namespace WingetNexus.Shared.Mappers
{
    public class InstallerMapper
    {
        public InstallerDto InstallerToInstallerDto(Installer installer)
        {
            return new InstallerDto
            {
                //Id = installer.Id,
                //Name = installer.Name,
                //Version = installer.Version,
                //Architecture = installer.Architecture,
                //Url = installer.Url,
                //Sha256 = installer.Sha256,
                //InstallerType = installer.InstallerType,
                //InstallerSwitches = installer.InstallerSwitches.Select(s => s.ToDto()).ToList(),
                //NestedInstallerFiles = installer.NestedInstallerFiles.Select(n => n.ToDto()).ToList()
            };
        }

        public IQueryable<InstallerDto> InstallerToDto(IQueryable<Installer> q)
        {
            return q.Select(i => InstallerToInstallerDto(i));
        }

        public Installer InstallerDtoToInstaller(InstallerDto installerForm)
        {
            return new Installer
            {
                //Id = installerForm.Id,
                //Name = installerForm.Name,
                //Version = installerForm.Version,
                //Architecture = installerForm.Architecture,
                //Url = installerForm.Url,
                //Sha256 = installerForm.Sha256,
                //InstallerType = installerForm.InstallerType,
                //InstallerSwitches = installerForm.InstallerSwitches.Select(s => s.ToEntity()).ToList(),
                //NestedInstallerFiles = installerForm.NestedInstallerFiles.Select(n => n.ToEntity()).ToList()
            };
        }
    }
}
