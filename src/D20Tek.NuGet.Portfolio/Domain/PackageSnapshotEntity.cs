namespace D20Tek.NuGet.Portfolio.Domain;

public sealed class PackageSnapshotEntity : IEntity
{
    public int Id { get; private set; }

    public DateOnly SnapshotDate { get; private set; }
    
    public long Downloads { get; private set; }

    public int TrackedPackageId { get; private set; }
    
    public TrackedPackageEntity TrackedPackage { get; private set; } = null!;

    private PackageSnapshotEntity() { }

    private PackageSnapshotEntity(int id, DateOnly snapshotDate, long downloads, TrackedPackageEntity trackedPackage)
    {
        Id = id;
        SnapshotDate = snapshotDate;
        Downloads = downloads;
        TrackedPackage = trackedPackage;
        TrackedPackageId = trackedPackage.Id;
    }

    public static PackageSnapshotEntity Create(long downloads, TrackedPackageEntity trackedPackage) =>
        new(0, DateOnly.FromDateTime(DateTimeOffset.Now.LocalDateTime), downloads, trackedPackage);

    public PackageSnapshotEntity ChangeDownloads(long downloads)
    {
        Downloads = downloads;
        return this;
    }
}