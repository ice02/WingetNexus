using WingetNexus.Data.DataStores;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;
using WingetNexus.Server.Mappers;

namespace WingetNexus.Server.Controllers;

// orig src https://github.com/berhir/BlazorWebAssemblyCookieAuth
[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private IApplicationDatastore _applicationDatastore;

    public UserController(IApplicationDatastore applicationDatastore)
    {
        _applicationDatastore = applicationDatastore;
    }

    [HttpGet("all")]
    //[Authorize(Roles = "Admin")]
    [Authorize]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = _applicationDatastore.GetAllUsers();
        return Ok(users.UserRoleToDto().ToList());
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult GetCurrentUser() => Ok(CreateUserInfo(User));

    private UserInfo CreateUserInfo(ClaimsPrincipal claimsPrincipal)
    {
        if (!claimsPrincipal?.Identity?.IsAuthenticated ?? true)
        {
            return UserInfo.Anonymous;
        }

        var userInfo = new UserInfo
        {
            IsAuthenticated = true
        };

        if (claimsPrincipal?.Identity is ClaimsIdentity claimsIdentity)
        {
            userInfo.NameClaimType = claimsIdentity.NameClaimType;
            userInfo.RoleClaimType = claimsIdentity.RoleClaimType;
        }
        else
        {
            userInfo.NameClaimType = ClaimTypes.Name;
            userInfo.RoleClaimType = ClaimTypes.Role;
        }

        if (claimsPrincipal?.Claims?.Any() ?? false)
        {
            // Add just the name claim
            //var claims = claimsPrincipal.FindAll(userInfo.NameClaimType)
            //                            .Select(u => new ClaimValue(userInfo.NameClaimType, u.Value))
            //                            .ToList();

            // Uncomment this code if you want to send additional claims to the client.
            var claims = claimsPrincipal.Claims.Select(u => new ClaimValue(u.Type, u.Value))
                                                  .ToList();
            userInfo.Claims = claims;

            var roles = _applicationDatastore.GetUserAcl(claims.FirstOrDefault(p => p.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value);

            userInfo.Roles = roles;
        }

        return userInfo;
    }
}
