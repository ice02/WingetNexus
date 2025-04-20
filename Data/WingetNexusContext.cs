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
            //Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Define primary key, relationships, and other configurations
            builder.Entity<Publisher>()
                .HasKey(e => e.Id);
            builder.Entity<Publisher>() // Replace 'NewEntity' with the actual entity name
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

            builder.Entity<Application>()
                .HasKey(e => e.Id);

            builder.Entity<Application>()
                .HasMany(e => e.Versions)
                .WithOne(e => e.Application)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Application>()
                .Property(e => e.Name)
                .IsRequired();

            builder.Entity<Application>()
                .Property(e => e.PackageIdentifier)
                .IsRequired();

            builder.Entity<Application>()
                .Property(e => e.GitHubUrl)
                .IsRequired(false);

            builder.Entity<Application>()
                .HasOne(e => e.Publisher)
                .WithMany(e => e.Applications)
                .HasForeignKey(e => e.PublisherId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);


            builder.Entity<Shared.Models.Entities.Version>()
                .HasKey(e => e.Id);
            builder.Entity<Shared.Models.Entities.Version>() // Replace 'NewEntity' with the actual entity name
                .HasOne(e => e.Application)
                .WithMany(e => e.Versions)
                .HasForeignKey(e => e.ApplicationId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Shared.Models.Entities.Version>()
                .Property(e => e.VersionNumber)
                .IsRequired();

            builder.Entity<Shared.Models.Entities.Version>()
                .Property(e => e.InstallersDatasJson)
                .IsRequired();

            builder.Entity<Shared.Models.Entities.Version>()
                .Property(e => e.LocalsDatasJson)
                .IsRequired();

            builder.Entity<Shared.Models.Entities.Version>()
                .Property(e => e.DefaultLocaleJson)
                .IsRequired();

            builder.Entity<Shared.Models.Entities.Version>()
                .Property(e => e.ManifestVersion)
                .IsRequired();

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

            // builder.Entity<WingetNexus.Data.Entities.Version>()
            //     .Property(e => e.JsonField) // Replace 'JsonField' with the actual property name
            //     .HasConversion(
            //         v => JsonConvert.SerializeObject(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
            //         v =>

            //             // Deserialize the JSON string based on the JsonVersion property value
            //             // based on the JsonVersion property value, calculate the full qualified class name
            //             //var typeName = $"WingetNexus.Shared.Models.Winget.{v.JsonVersion}.{nameof(NewEntity)}"; // Replace 'NewEntity' with the actual entity name
            //             JsonConvert.DeserializeObject<dynamic>(v, new JsonSerializerSettings
            //             {
            //                 NullValueHandling = NullValueHandling.Ignore
            //                 //Converters = { new VersionedJsonConverter(v.JsonVersion) } // Use the JsonVersion property value
            //             })
            //     );

            // Add other entity configurations as needed
        }

        // Define DbSet properties for the new entities
        public DbSet<Shared.Models.Entities.Version> Versions { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<Locale> Locales { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<TutorialDismissedState> TutorialDismissedStates { get; set; }
    }
}
