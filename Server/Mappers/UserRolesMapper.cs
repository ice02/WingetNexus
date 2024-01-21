using Riok.Mapperly.Abstractions;

namespace WingetNexus.Server.Mappers
{
    [Mapper(UseDeepCloning = true)]
    public static partial class UserRolesMapper
    {
        public static partial Shared.Models.Dtos.UserRoleDto UserRoleToUserRoleDto(this Data.Models.UserRole userRole);
        public static partial System.Linq.IQueryable<Shared.Models.Dtos.UserRoleDto> UserRoleToDto(this System.Linq.IQueryable<Data.Models.UserRole> q);
        public static partial Data.Models.UserRole UserRoleDtoToUserRole(this Shared.Models.Dtos.UserRoleDto userRoleForm);
    }
}
