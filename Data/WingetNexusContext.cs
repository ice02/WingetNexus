using Microsoft.EntityFrameworkCore;
using WingetNexus.Shared.Models.Entities;
using WingetNexus.Data.Models;

namespace WingetNexus.Data
{
    // Entity framework context for the new entities.
    public class WingetNexusContext : DbContext
    {
        public WingetNexusContext(DbContextOptions<WingetNexusContext> options)
            : base(options)
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Define primary key, relationships, and other configurations
            builder.Entity<Publisher>()
                .HasKey(e => e.Id);
            builder.Entity<Publisher>()
                .HasMany(e => e.Applications)
                .WithOne(e => e.Publisher)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Publisher>()
                .Property(e => e.Name)
                .IsRequired();

            builder.Entity<Publisher>()
                .Property(e => e.GitHubUrl)
                .IsRequired(false);

            builder.Entity<Application>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasMany(e => e.Versions)
                        .WithOne(e => e.Application)
                        .IsRequired()
                        .OnDelete(DeleteBehavior.Cascade);

                entity.Property(e => e.Name)
                        .IsRequired();

                entity.Property(e => e.PackageIdentifier)
                        .IsRequired();

                entity.Property(e => e.GitHubUrl)
                        .IsRequired(false);

                entity.HasOne(e => e.Publisher)
                        .WithMany(e => e.Applications)
                        .HasForeignKey(e => e.PublisherId)
                        .IsRequired()
                        .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Shared.Models.Entities.Version>(e =>
            {
                e.HasKey(v => v.Id);
                e.Property(v => v.VersionNumber).IsRequired();
                e.Property(v => v.DefaultLocaleValue).IsRequired();
                e.HasOne(v => v.Application)
                    .WithMany(a => a.Versions)
                    .HasForeignKey(v => v.ApplicationId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
                e.Property(v => v.CreatedDate).IsRequired();
                e.Property(v => v.ModifiedDate).IsRequired();
                e.Property(v => v.UserCreated).IsRequired();
                e.Property(v => v.UserLastModified).IsRequired();
                e.Property(v => v.ShortDescription).IsRequired();
                e.Property(v => v.ManifestVersion).IsRequired();
                // Relationship with ContentFiles for VersionContent
                e.HasOne(v => v.VersionContent)
                    .WithOne(c => c.VersionContent)
                    .HasForeignKey<Shared.Models.Entities.Version>(c => c.VersionContentFK)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relationship with ContentFiles for InstallersContent
                e.HasOne(v => v.InstallersContent)
                    .WithOne(c => c.VersionContentForInstaller)
                    .HasForeignKey<Shared.Models.Entities.Version>(c => c.InstallersContentFK)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relationship with ContentFiles for DefaultLocaleContent
                e.HasOne(v => v.DefaultLocaleContent)
                    .WithOne(c => c.VersionContentForDefaultLocale)
                    .HasForeignKey<Shared.Models.Entities.Version>(c => c.DefaultLocaleFK)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relationship with ContentFiles for LocalesContent
                e.HasMany(v => v.LocalesContent)
                    .WithOne(c => c.VersionContentForLocal)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Locale>()
                .HasKey(e => e.Id);

            builder.Entity<Locale>()
                .Property(e => e.PackageLocale)
                .IsRequired();

            builder.Entity<Locale>()
                .Property(e => e.JsonContent)
                .IsRequired();

            builder.Entity<Locale>()
                .Property(e => e.JsonVersion)
                .IsRequired();

            builder.Entity<ContentFiles>()
                .HasKey(e => e.Id);

            builder.Entity<Command>().HasKey(c => c.RowId);
            builder.Entity<CommandMap>().HasKey(cm => new { cm.Command, cm.Package });
            builder.Entity<Metadata>().HasKey(m => m.Name);
            builder.Entity<NormName>().HasKey(nn => new { nn.NormName, nn.Package });
            builder.Entity<NormPublisher>().HasKey(np => new { np.NormPublisher, np.Package });
            builder.Entity<Pfn>().HasKey(p => new { p.Pfn, p.Package });
            builder.Entity<ProductCode>().HasKey(pc => new { pc.ProductCode, pc.Package });
            builder.Entity<TagMap>().HasKey(tm => new { tm.Tag, tm.Package });
            builder.Entity<UpgradeCode>().HasKey(uc => new { uc.UpgradeCode, uc.Package });
        }

        // Define DbSet properties for the new entities
        public DbSet<Shared.Models.Entities.Version> Versions { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<Locale> Locales { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<ContentFiles> ContentFiles { get; set; }
        public DbSet<TutorialDismissedState> TutorialDismissedStates { get; set; }

        public DbSet<Command> Commands2 { get; set; }
        public DbSet<CommandMap> Commands2Map { get; set; }
        public DbSet<Metadata> Metadata { get; set; }
        public DbSet<NormName> NormNames2 { get; set; }
        public DbSet<NormPublisher> NormPublishers2 { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<Pfn> Pfns2 { get; set; }
        public DbSet<ProductCode> ProductCodes2 { get; set; }
        public DbSet<Tag> Tags2 { get; set; }
        public DbSet<TagMap> Tags2Map { get; set; }
        public DbSet<UpgradeCode> UpgradeCodes2 { get; set; }
    }

    public class Command
    {
        public int RowId { get; set; }
        public string CommandText { get; set; }
    }

    public class CommandMap
    {
        public long Command { get; set; }
        public long Package { get; set; }
    }

    public class Metadata
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class NormName
    {
        public string NormName { get; set; }
        public long Package { get; set; }
    }

    public class NormPublisher
    {
        public string NormPublisher { get; set; }
        public long Package { get; set; }
    }

    public class Package
    {
        public int RowId { get; set; }
        public long Id { get; set; }
        public long Name { get; set; }
        public long? Moniker { get; set; }
        public long LatestVersion { get; set; }
        public long? ArpMinVersion { get; set; }
        public long? ArpMaxVersion { get; set; }
        public long? Hash { get; set; }
    }

    public class Pfn
    {
        public string Pfn { get; set; }
        public long Package { get; set; }
    }

    public class ProductCode
    {
        public string ProductCode { get; set; }
        public long Package { get; set; }
    }

    public class Tag
    {
        public int RowId { get; set; }
        public string TagText { get; set; }
    }

    public class TagMap
    {
        public long Tag { get; set; }
        public long Package { get; set; }
    }

    public class UpgradeCode
    {
        public string UpgradeCode { get; set; }
        public long Package { get; set; }
    }
}
