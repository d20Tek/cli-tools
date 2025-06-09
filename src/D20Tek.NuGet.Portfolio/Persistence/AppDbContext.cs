using D20Tek.NuGet.Portfolio.Domain;
using D20Tek.NuGet.Portfolio.Persistence.Configurations;
using HabitTracker.Api.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace D20Tek.NuGet.Portfolio.Persistence;

internal sealed class AppDbContext : DbContext
{
    public DbSet<CollectionEntity> Collections { get; set; }

    public DbSet<TrackedPackageEntity> TrackedPackages { get; set; }

    public DbSet<PackageSnapshotEntity> PackageSnapshots { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfiguration(new CollectionConfiguration())
                    .ApplyConfiguration(new TrackedPackageConfiguration())
                    .ApplyConfiguration(new PackageSnapshotConfiguration());
}
