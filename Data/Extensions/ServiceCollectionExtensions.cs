using EFCoreSecondLevelCacheInterceptor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WingetNexus.Data.Extensions
{
    public static class ServiceCollectionExtensions
    {
        //public static IServiceCollection AddSqlServerWithCache<T>(
        //this IServiceCollection services,
        //string connectionString,
        //bool allowCache = true,
        //bool splitQuery = true)
        //where T : DbContext
        //{
        //    services.AddDbContextPool<T>((serviceProvider, optionsBuilder) =>
        //        optionsBuilder
        //            .UseSqlServer(
        //                connectionString: connectionString,
        //                sqlServerOptionsAction: options =>
        //                {
        //                    if (splitQuery)
        //                    {
        //                        options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        //                    }
        //                    options.EnableRetryOnFailure();
        //                    options.CommandTimeout(30);
        //                })
        //            .AddInterceptors(serviceProvider.GetRequiredService<SecondLevelCacheInterceptor>()));

        //    services.AddEFSecondLevelCache(options =>
        //    {
        //        if (allowCache)
        //        {
        //            options.UseMemoryCacheProvider().DisableLogging(true);
        //            options.CacheAllQueries(CacheExpirationMode.Sliding, TimeSpan.FromMinutes(30));
        //        }
        //    });
        //    return services;
        //}

        public static IServiceCollection AddPGSqlWithCache<T>(
        this IServiceCollection services,
        string connectionString,
        bool allowCache = true,
        bool splitQuery = true)
        where T : DbContext
        {
            services.AddDbContextPool<T>((serviceProvider, optionsBuilder) =>
                optionsBuilder
                    .UseNpgsql(
                        connectionString: connectionString

                    //,sqlServerOptionsAction: options =>
                    //{
                    //    if (splitQuery)
                    //    {
                    //        options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                    //    }
                    //    options.EnableRetryOnFailure();
                    //    options.CommandTimeout(30);
                    //}
                    )
                    .AddInterceptors(serviceProvider.GetRequiredService<SecondLevelCacheInterceptor>()));

            services.AddEFSecondLevelCache(options =>
            {
                if (allowCache)
                {
                    options.UseMemoryCacheProvider().DisableLogging(true);
                    options.CacheAllQueries(CacheExpirationMode.Sliding, TimeSpan.FromMinutes(30));
                }
            });
            return services;
        }


        public static IServiceCollection AddInMemoryDb<T>(
            this IServiceCollection services)
            where T : DbContext
        {
            return services.AddDbContext<T>(optionsBuilder =>
            {
                optionsBuilder
                    .UseInMemoryDatabase(databaseName: "in-memory");
            });
        }

        public static IServiceCollection AddSqliteWithCache<T>(
            this IServiceCollection services,
            string connectionString,
            bool allowCache = true,
            bool splitQuery = true)
            where T : DbContext
        {
            services.AddDbContextPool<T>((serviceProvider, optionsBuilder) =>
            {
                optionsBuilder
                    .UseSqlite(
                        connectionString: connectionString,
                        sqliteOptionsAction: options =>
                        {
                            if (splitQuery)
                            {
                                options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                            }
                            options.CommandTimeout(30);
                        })
                    .AddInterceptors(serviceProvider.GetRequiredService<SecondLevelCacheInterceptor>());
            });

            services.AddEFSecondLevelCache(options =>
            {
                if (allowCache)
                {
                    options.UseMemoryCacheProvider().DisableLogging(true);
                    options.CacheAllQueries(CacheExpirationMode.Sliding, TimeSpan.FromMinutes(30));
                }
            });

            return services;
        }

    }
}
