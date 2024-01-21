using Microsoft.AspNetCore.Mvc.Filters;
using WingetNexus.Data;
using WingetNexus.Data.DataStores;

namespace WingetNexus.Server.Security
{
    public class DatabasePolicyAuthorizationHandler : AuthorizationHandler<DatabasePolicyRequirement>
    {
        readonly IApplicationDatastore _applicationDatastore;
        //readonly IHttpContextAccessor _contextAccessor;

        public DatabasePolicyAuthorizationHandler(IApplicationDatastore c)
        {
            //_contextAccessor = ca;
            _applicationDatastore = c;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, DatabasePolicyRequirement requirement)
        {
            if (context.Resource is AuthorizationFilterContext filterContext)
            {
                //var area = (filterContext.RouteData.Values["area"] as string)?.ToLower();
                var controller = (filterContext.RouteData.Values["controller"] as string)?.ToLower();
                var action = (filterContext.RouteData.Values["action"] as string)?.ToLower();
                //var id = (filterContext.RouteData.Values["id"] as string)?.ToLower();
                if (await requirement.Pass(_applicationDatastore, context, controller, action))
                {
                    context.Succeed(requirement);
                }
            }
            if (context.Resource is DefaultHttpContext httpContext)
            {
                //var area = httpContext.Request.RouteValues["area"].ToString();
                var controller = httpContext.Request.RouteValues["controller"].ToString();
                var action = httpContext.Request.RouteValues["action"].ToString();
                //var id = httpContext.Request.RouteValues["id"].ToString();
                if (await requirement.Pass(_applicationDatastore, context, controller, action))
                {
                    context.Succeed(requirement);
                }
            }
        }
    }
}
