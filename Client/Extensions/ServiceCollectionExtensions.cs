namespace WingetNexus.Client.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddSingleton<IGithubService, GithubService>();
            services.AddSingleton<IYamlFilesService, YamlFilesService>();
            services.AddSingleton<IApplicationService, ApplicationService>();
            services.AddSingleton<IPackageService, PackageService>();

            return services;
        }
    }
}
