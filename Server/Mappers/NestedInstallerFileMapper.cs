using Riok.Mapperly.Abstractions;
using WingetNexus.Shared.Models.Db;
using WingetNexus.Shared.Models.Db.Objects;
using WingetNexus.Shared.Models.Dtos;

namespace WingetNexus.Server.Mappers
{
    [Mapper(UseDeepCloning = true)]
    public static partial class NestedInstallerFileMapper
    {
        public static partial NestedInstallerFileDto NestedInstallerFileToNestedInstallerFileDto(this NestedInstallerFile installer);
        public static partial IQueryable<InstallerDto> NestedInstallerFileToDto(this IQueryable<NestedInstallerFile> q);
        public static partial NestedInstallerFile NestedInstallerFileDtoToInstaller(this InstallerDto installerForm);
    }
}
