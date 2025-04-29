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
                    .HasForeignKey<Shared.Models.Entities.Version>(c => c.VersionContentFK) // Adjust foreign key if necessary
                    .OnDelete(DeleteBehavior.Cascade);

                // Relationship with ContentFiles for InstallersContent
                e.HasOne(v => v.InstallersContent)
                    .WithOne(c => c.VersionContentForInstaller)
                    .HasForeignKey<Shared.Models.Entities.Version>(c => c.InstallersContentFK) // Adjust foreign key if necessary
                    .OnDelete(DeleteBehavior.Cascade);

                // Relationship with ContentFiles for DefaultLocaleContent
                e.HasOne(v => v.DefaultLocaleContent)
                    .WithOne(c => c.VersionContentForDefaultLocale)
                    .HasForeignKey<Shared.Models.Entities.Version>(c => c.DefaultLocaleFK) // Adjust foreign key if necessary
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
        public DbSet<ContentFiles> ContentFiles { get; set; }
        public DbSet<TutorialDismissedState> TutorialDismissedStates { get; set; }
    }
}
