using Microsoft.AspNetCore.Mvc.Filters;

namespace WingetNexus.Server.Security
{
    public class NexusAuthorizeAttribute : AuthorizeAttribute
    {
        public NexusAuthorizeAttribute(string permission)
        {
            Policy = "DatabasePolicy";
        }

        public string? Controller { get; set; }
        public string? Action { get; set; }
    }
}
