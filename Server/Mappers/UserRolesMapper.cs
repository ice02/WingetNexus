using Riok.Mapperly.Abstractions;

namespace WingetNexus.Shared.Mappers.AutoMapperProfiles
{
    [Mapper(UseDeepCloning = true)]
    public static partial class UserRolesMapper
    {
        public static partial Models.Dtos.UserRoleDto UserRoleToUserRoleDto(this Data.Models.UserRole userRole);
        public static partial IQueryable<Models.Dtos.UserRoleDto> UserRoleToDto(this IQueryable<Data.Models.UserRole> q);
        public static partial Data.Models.UserRole UserRoleDtoToUserRole(this Models.Dtos.UserRoleDto userRoleForm);
    }
}
