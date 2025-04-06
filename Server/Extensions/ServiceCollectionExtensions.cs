namespace WingetNexus.Server.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAutoMapper(this IServiceCollection services, Type[] ProfilesAssemblies)
        {
            services.AddAutoMapper(ProfilesAssemblies);
            return services;
        }
    }
}
