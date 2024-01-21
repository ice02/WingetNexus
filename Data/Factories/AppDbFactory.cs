//using Microsoft.Extensions.Configuration;
//using SQLitePCL;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace WingetNexus.Data.Factories
//{
//    public interface IAppDbFactory
//    {
//        AppDbContext GetContext();
//    }

//    public class AppDbFactory : IAppDbFactory
//    {
//        private readonly IApplicationConfig _appConfig;
//        private readonly IConfiguration _configuration;
//        private readonly IWebHostEnvironment _hostEnv;

//        public AppDbFactory(
//            IApplicationConfig appConfig,
//            IConfiguration configuration,
//            IWebHostEnvironment hostEnv)
//        {
//            _appConfig = appConfig;
//            _configuration = configuration;
//            _hostEnv = hostEnv;
//        }

//        public AppDbContext GetContext()
//        {
//            return _appConfig.DBProvider.ToLower() switch
//            {
//                "sqlite" => new SqliteDbContext(_configuration, _hostEnv),
//                "sqlserver" => new SqlServerDbContext(_configuration, _hostEnv),
//                "postgresql" => new PostgreSqlDbContext(_configuration, _hostEnv),
//                "inmemory" => new TestingDbContext(_hostEnv),
//                _ => throw new ArgumentException("Unknown DB provider."),
//            };
//        }
//    }
