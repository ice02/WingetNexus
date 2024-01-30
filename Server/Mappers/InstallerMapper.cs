using Riok.Mapperly.Abstractions;
using WingetNexus.Shared.Models.Db;
using WingetNexus.Shared.Models.Dtos;

namespace WingetNexus.Server.Mappers
{
    [Mapper(UseDeepCloning = true)]
    public static partial class InstallerMapper
    {
        public static partial InstallerDto InstallerToInstallerDto(this Installer installer);
        public static partial IQueryable<InstallerDto> InstallerToDto(this IQueryable<Installer> q);
        public static partial Installer InstallerDtoToInstaller(this InstallerDto installerForm);
    }
}
