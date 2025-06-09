using D20Tek.NuGet.Portfolio.Domain;
using D20Tek.NuGet.Portfolio.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace D20Tek.NuGet.Portfolio.Persistence;

internal sealed class AppDbContext : DbContext
{
    public DbSet<CollectionEntity> Collections { get; set; }

    public DbSet<TrackedPackageEntity> TrackedPackages { get; set; }

    public DbSet<PackageSnapshotEntity> PackageSnapshots { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfiguration(new CollectionConfiguration())
                    .ApplyConfiguration(new TrackedPackageConfiguration())
                    .ApplyConfiguration(new PackageSnapshotConfiguration());

    internal void ApplyMigrations(IAnsiConsole console)
    {
        try
        {
            if (Database.GetPendingMigrations().Any())
            {
                console.WriteLine("Checking for pending migrations...");
                Database.Migrate();
                console.WriteLine("Database is up to date.");
            }
        }
        catch (Exception ex)
        {
            console.WriteLine($"Migration failed: {ex.Message}");
            throw;
        }
    }
}
