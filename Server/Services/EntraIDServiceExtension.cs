using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace WingetNexus.Server.Services
{
    public static class EntraIDServiceExtension
    {
        public static IServiceCollection AddEntraIDAutentication(this IServiceCollection services)
        {
            services.AddScoped<MsGraphService>();

            //TODO: add all middleware for entraid authentication

            return services;
        }

        //middleware to add all services to the DI container

    }
}
