using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WingetNexus.Data.Models;

namespace WingetNexus.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            Database.EnsureCreated();
            //EnableSensitiveDataLogging = true;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRole>()
                .HasIndex(p => new { p.UserId, p.RoleName })
                .IsUnique();
            
            base.OnModelCreating(modelBuilder);
        }

        //public DbSet<Register> Registration { get; set; } = default!;
        //public DbSet<TokenInfo> TokenInfo { get; set; } = default!;
        public DbSet<UserRole> UserRoles { get; set; } = default!;
    }

    //public class ApiAuthorizationDbContext<ApplicationUser> : IdentityDbContext<ApplicationUser>
    //{
    //    public ApiAuthorizationDbContext(DbContextOptions options, IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options)
    //    {
    //    }
    //}

    //public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    //{
    //}

    //public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
    //{
    //    public ApplicationDbContext(
    //        DbContextOptions options,
    //        IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
    //    {

    //    }
    //    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    //    public DbSet<IdentityRole> IdentityRoles { get; set; }
    //    public DbSet<IdentityRoleClaim<string>> IdentityRoleClaims { get; set; }
    //    public DbSet<IdentityUserRole<string>> IdentityUserRoles { get; set; }
    //}
}
