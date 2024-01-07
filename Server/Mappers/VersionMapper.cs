using Riok.Mapperly.Abstractions;
using WingetNexus.Shared.Models.Db;
using WingetNexus.Shared.Models.Dtos;

namespace WingetNexus.Server.Mappers
{
    [Mapper(UseDeepCloning = true)]
    public static partial class VersionMapper
    {
        public static partial VersionDto VersionToVersionDto(this PackageVersion version);
        public static partial IQueryable<VersionDto> VersionToDto(this IQueryable<PackageVersion> q);
        public static partial PackageVersion VersionDtoToVersion(this VersionDto versionForm);
    }
}
