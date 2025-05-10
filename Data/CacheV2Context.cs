using Microsoft.EntityFrameworkCore;

namespace WingetNexus.Data
{
    public class CacheV2Context : DbContext
    {
        public CacheV2Context(DbContextOptions<CacheV2Context> options) : base(options) { }

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Command>().HasKey(c => c.RowId);
            modelBuilder.Entity<CommandMap>().HasKey(cm => new { cm.Command, cm.Package });
            modelBuilder.Entity<Metadata>().HasKey(m => m.Name);
            modelBuilder.Entity<NormName>().HasKey(nn => new { nn.NormName, nn.Package });
            modelBuilder.Entity<NormPublisher>().HasKey(np => new { np.NormPublisher, np.Package });
            modelBuilder.Entity<Pfn>().HasKey(p => new { p.Pfn, p.Package });
            modelBuilder.Entity<ProductCode>().HasKey(pc => new { pc.ProductCode, pc.Package });
            modelBuilder.Entity<TagMap>().HasKey(tm => new { tm.Tag, tm.Package });
            modelBuilder.Entity<UpgradeCode>().HasKey(uc => new { uc.UpgradeCode, uc.Package });
        }
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