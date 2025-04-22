using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WingetNexus.Shared.Mappers;
using WingetNexus.Shared.Mappers.AutoMapperProfiles;

namespace WingetNexus.Shared.Services
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddDataMappers(this IServiceCollection services)
        {
            services.AddSingleton<VersionMapper>();
            services.AddSingleton<ApplicationMapper>();
            services.AddSingleton<InstallerMapper>();

            //add auto mappers
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<VersionProfile>();
                cfg.AddProfile<ApplicationProfile>();
                cfg.AddProfile<PublisherProfile>();
            }, typeof(ServiceCollectionExtension).Assembly);


            return services;
        }
    }
}
