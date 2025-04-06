using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


namespace WingetNexus.Data
{
    public class WingetNexusContextSQLite : WingetNexusContext
    {
        private readonly IConfiguration _configuration;

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite(_configuration.GetConnectionString("WingetSqLiteContext"));

        public WingetNexusContextSQLite(DbContextOptions<WingetNexusContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }
    }
}
