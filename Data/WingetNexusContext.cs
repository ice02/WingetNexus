using Microsoft.EntityFrameworkCore;
using System.Net;
using System;
using WingetNexus.Data.Models;
using WingetNexus.Shared.Models.Db;
using WingetNexus.Shared.Models.Db.Objects;

namespace WingetNexus.Data
{
    // Entity framwork context for the SQLite database.
    public class WingetNexusContext : DbContext, IWingetDatasource
    {
        public WingetNexusContext(DbContextOptions<WingetNexusContext> options)
            : base(options)
        {
            Database.EnsureCreated();
            //EnableSensitiveDataLogging = true;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Package>()
                .HasMany(p => p.Versions).WithOne(p=>p.Package).IsRequired().OnDelete(DeleteBehavior.Cascade);

            builder.Entity<PackageVersion>()
                .HasOne(p => p.Package)
                .WithMany(p => p.Versions)
                .IsRequired().OnDelete(DeleteBehavior.Cascade);

            builder.Entity<PackageVersion>()
                .HasOne(p => p.DefaultLocale);

            //builder.Entity<PackageVersion>()
            //    .HasMany(p => p.Locales);

            builder.Entity<Installer>()
                .HasMany(p => p.Switches)
                .WithOne(p => p.Installer)
                .IsRequired().OnDelete(DeleteBehavior.Cascade);

            builder.Entity<InstallerSwitch>()
                .HasOne(p => p.Installer)
                .WithMany(p => p.Switches)
                .IsRequired().OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Installer>()
                .HasMany(p => p.NestedInstallerFiles).WithOne(p => p.Installer)
                .IsRequired().OnDelete(DeleteBehavior.Cascade);

            builder.Entity<NestedInstallerFile>()
                .HasOne(p => p.Installer)
                .WithMany(p => p.NestedInstallerFiles)
                .IsRequired().OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Package>()
                .HasIndex(p => p.Identifier)
                .IsUnique();

            builder.Entity<PackageVersion>()
                .HasIndex(p => new { p.PackageId, p.VersionCode })
                .IsUnique();

            builder.Entity<Package>()
                .HasIndex(p => p.Name);

            builder.Entity<Package>()
                .HasIndex(p => p.Publisher);

            builder.Entity<Locale>()
                .HasKey(l => l.PackageLocale);

        }

        public DbSet<Package> Packages { get; set; }
        public DbSet<PackageVersion> PackageVersions { get; set; }
        public DbSet<Installer> Installers { get; set; }
        public DbSet<InstallerSwitch> InstallerSwitches { get; set; }
        public DbSet<NestedInstallerFile> NestedInstallerFiles { get; set; }
        public DbSet<Locale> Locales { get; set; }

        public DbSet<Register> Registration { get; set; } = default!;
        public DbSet<TokenInfo> TokenInfo { get; set; } = default!;
        public DbSet<UserRole> UserRoles { get; set; } = default!;
    }
    
}
