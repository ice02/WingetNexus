using EFCoreSecondLevelCacheInterceptor;
using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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

            //services.AddEFSecondLevelCache(options =>
            //{
            //    if (allowCache)
            //    {
            //        options.UseMemoryCacheProvider().DisableLogging(true);
            //        options.CacheAllQueries(CacheExpirationMode.Sliding, TimeSpan.FromMinutes(30));
            //    }
            //});

            
            if (allowCache)
            {
                const string providerName1 = "Redis1";

                services.AddEFSecondLevelCache(options =>
                        options.UseEasyCachingCoreProvider(providerName1, isHybridCache: false)
                        .DisableLogging(true)
                        .UseCacheKeyPrefix("EF_")
                        // Fallback on db if the caching provider fails (for example, if Redis is down).
                        .UseDbCallsIfCachingProviderIsDown(TimeSpan.FromMinutes(1))
                );


                // More info: https://easycaching.readthedocs.io/en/latest/Redis/
                services.AddEasyCaching(option =>
                {
                    option.UseRedis(config =>
                    {
                        config.DBConfig.AllowAdmin = true;
                        config.DBConfig.SyncTimeout = 10000;
                        config.DBConfig.AsyncTimeout = 10000;
                        config.DBConfig.Endpoints.Add(new EasyCaching.Core.Configurations.ServerEndPoint("127.0.0.1", 6379));
                        config.EnableLogging = true;
                        config.SerializerName = "Pack";
                        config.DBConfig.ConnectionTimeout = 10000;
                    }, providerName1)
                    .WithMessagePack(so =>
                    {
                        so.EnableCustomResolver = true;
                        so.CustomResolvers = CompositeResolver.Create(
                            new IMessagePackFormatter[]
                            {
                            DbNullFormatter.Instance, // This is necessary for the null values
                            },
                            new IFormatterResolver[]
                            {
                            NativeDateTimeResolver.Instance,
                            ContractlessStandardResolver.Instance,
                            StandardResolverAllowPrivate.Instance,
                            }
                        );
                    },
                    "Pack");
                });
            }

            return services;
        }

    }

    public class DbNullFormatter : IMessagePackFormatter<DBNull>
    {
        public static DbNullFormatter Instance = new();

        private DbNullFormatter()
        {
        }

        public void Serialize(ref MessagePackWriter writer, DBNull value, MessagePackSerializerOptions options)
        {
            // always serialize as nil (if present, it's never null)
            writer.WriteNil();
        }

        public DBNull Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            return DBNull.Value;
        }
    }
}
