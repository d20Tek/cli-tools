using D20Tek.NuGet.Portfolio.Domain;
using D20Tek.NuGet.Portfolio.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace D20Tek.NuGet.Portfolio.Persistence;

internal sealed class AppDbContext(DbContextOptions<AppDbContext> options, ILogger<AppDbContext> logger) :
    DbContext(options)
{
    private readonly ILogger<AppDbContext> _logger = logger;

    public DbSet<CollectionEntity> Collections { get; set; }

    public DbSet<TrackedPackageEntity> TrackedPackages { get; set; }

    public DbSet<PackageSnapshotEntity> PackageSnapshots { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfiguration(new CollectionConfiguration())
                    .ApplyConfiguration(new TrackedPackageConfiguration())
                    .ApplyConfiguration(new PackageSnapshotConfiguration());

    internal void ApplyMigrations()
    {
        try
        {
            if (Database.GetPendingMigrations().Any())
            {
                _logger.LogInformation("Performing database changes...");
                Database.Migrate();
                _logger.LogInformation("Database ready.");
                _logger.LogInformation("Seeding intial data.");
                SeedRequiredData();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Migration failed: {ex.Message}");
            throw;
        }
    }

    private void SeedRequiredData()
    {
        if (Collections.Any() is false)
        {
            CollectionEntity.Create("Default")
                            .Iter(c =>
                            {
                                Collections.Add(c);
                                SaveChanges();
                            });
        }
    }
}
