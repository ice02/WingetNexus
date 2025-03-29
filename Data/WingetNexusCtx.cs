using Microsoft.EntityFrameworkCore;

namespace WingetNexus.Data
{
    // Entity framework context for the new entities.
    public class WingetNexusCtx : DbContext
    {
        public WingetNexusCtx(DbContextOptions<WingetNexusCtx> options)
            : base(options)
        {
            //Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Define primary key, relationships, and other configurations
            builder.Entity<WingetNexus.Shared.Entities.Publisher>() 
                .HasKey(e => e.Id);
            builder.Entity<WingetNexus.Shared.Entities.Publisher>() // Replace 'NewEntity' with the actual entity name
                .HasMany(e => e.Applications)
                .WithOne(e => e.Publisher)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<WingetNexus.Shared.Entities.Publisher>()
                .Property(e => e.Name)
                .IsRequired();
            
            builder.Entity<WingetNexus.Shared.Entities.Publisher>()
                .Property(e => e.GitHubUrl)
                .IsRequired(false);

            builder.Entity<WingetNexus.Shared.Entities.Application>()
                .HasKey(e => e.Id);

            builder.Entity<WingetNexus.Shared.Entities.Application>() 
                .HasMany(e => e.Versions)
                .WithOne(e => e.Application)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<WingetNexus.Shared.Entities.Application>()
                .Property(e => e.Name)
                .IsRequired();

            builder.Entity<WingetNexus.Shared.Entities.Application>()
                .Property(e => e.PackageIdentifier)
                .IsRequired();

            builder.Entity<WingetNexus.Shared.Entities.Application>()
                .Property(e => e.GitHubUrl)
                .IsRequired(false);

            builder.Entity<WingetNexus.Shared.Entities.Application>()
                .HasOne(e => e.Publisher)
                .WithMany(e => e.Applications)
                .HasForeignKey(e => e.PublisherId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            
            builder.Entity<WingetNexus.Shared.Entities.Version>() 
                .HasKey(e => e.Id);
            builder.Entity<WingetNexus.Shared.Entities.Version>() // Replace 'NewEntity' with the actual entity name
                .HasOne(e => e.Application)
                .WithMany(e => e.Versions)
                .HasForeignKey(e => e.ApplicationId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<WingetNexus.Shared.Entities.Version>()
                .Property(e => e.VersionNumber)
                .IsRequired();

            builder.Entity<WingetNexus.Shared.Entities.Version>()
                .Property(e => e.DatasJson)
                .IsRequired();                
                
            builder.Entity<WingetNexus.Shared.Entities.Version>()
                .Property(e => e.ManifestVersion)
                .IsRequired();

            builder.Entity<WingetNexus.Shared.Entities.Version>()
                .Property(e => e.DefaultLocale)
                .IsRequired();

            builder.Entity<WingetNexus.Shared.Entities.Locale>()
                .HasKey(e => e.Id);

            builder.Entity<WingetNexus.Shared.Entities.Locale>()
                .Property(e => e.PackageLocale)
                .IsRequired();

            builder.Entity<WingetNexus.Shared.Entities.Locale>()
                .Property(e => e.JsonContent)
                .IsRequired();

            builder.Entity<WingetNexus.Shared.Entities.Locale>()
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
        public DbSet<WingetNexus.Shared.Entities.Version> Versions { get; set; } 
        public DbSet<WingetNexus.Shared.Entities.Application> Applications { get; set; }
        public DbSet<WingetNexus.Shared.Entities.Locale> Locales { get; set; }
        public DbSet<WingetNexus.Shared.Entities.Publisher> Publishers { get; set; }
    }
}
