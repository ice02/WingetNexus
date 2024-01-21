using WingetNexus.Data;
using WingetNexus.Data.DataStores;

namespace WingetNexus.Server.Security
{
    public class DatabasePolicyRequirement : IAuthorizationRequirement
    {
        AuthorizationHandlerContext _contextAccessor;
        string[] RoleName { get; set; }

        public DatabasePolicyRequirement(string[] roleName)
        {
            RoleName = roleName;
        }

        public async Task<bool> Pass(IApplicationDatastore applicationDatastore, AuthorizationHandlerContext contextAccessor, string controller, string action)
        {
            _contextAccessor = contextAccessor;

            //authorization logic goes here
            var user = _contextAccessor.User;
            if (user.Identity.IsAuthenticated)
            {
                var roles = applicationDatastore.GetUserAcl(user.Identity.Name);
                foreach(var role in RoleName)
                {
                    if (roles.Contains(role))
                    {
                        return await Task.FromResult(true);
                    }
                }
            }

            return await Task.FromResult(false);
        }
    }
}
