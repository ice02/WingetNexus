using Riok.Mapperly.Abstractions;
using WingetNexus.Shared.Models.Db;
using WingetNexus.Shared.Models.Dtos;

namespace WingetNexus.Server.Mappers
{
    [Mapper(UseDeepCloning = true)]
    public static partial class SwitchMapper
    {
        public static partial SwitchDto SwitchToSwitchDto(this InstallerSwitch sw);
        public static partial IQueryable<SwitchDto> SwitchToDto(this IQueryable<InstallerSwitch> q);
        public static partial InstallerSwitch SwitchDtoToSwitch(this SwitchDto swForm);
    }
}
