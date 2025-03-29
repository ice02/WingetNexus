using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WingetNexus.Data
{
    public class WingetNexusCtxPGSql : WingetNexusCtx
    {
        private readonly IConfiguration _configuration;

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseNpgsql(_configuration.GetConnectionString("WingetPGSqlContext"));

        public WingetNexusCtxPGSql(DbContextOptions<WingetNexusCtx> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }
    }
}
