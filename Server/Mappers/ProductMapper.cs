using Riok.Mapperly.Abstractions;
using WingetNexus.Shared.Models.Db;
using WingetNexus.Shared.Models.Dtos;
using static MudBlazor.CategoryTypes;

namespace WingetNexus.Server.Mappers
{
    // Mapper declaration
    [Mapper(UseDeepCloning = true)]
    public static partial class PackageMapper
    {
        public static partial PackageDto PackageToPackageDto(this Package car);
        public static partial IQueryable<PackageDto> ProjectToDto(this IQueryable<Package> q);
    }
}
