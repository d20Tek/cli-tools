namespace D20Tek.NuGet.Portfolio.Domain;

public sealed class PackageSnapshotEntity : IEntity
{
    public int Id { get; private set; }

    public DateTime SnapshotDate { get; private set; }
    
    public long Downloads { get; private set; }

    public int TrackedPackageId { get; private set; }
    
    public TrackedPackageEntity TrackedPackage { get; private set; } = null!;

    private PackageSnapshotEntity() { }

    private PackageSnapshotEntity(int id, DateTime snapshotDate, long downloads, TrackedPackageEntity trackedPackage)
    {
        Id = id;
        SnapshotDate = snapshotDate;
        Downloads = downloads;
        TrackedPackage = trackedPackage;
        TrackedPackageId = trackedPackage.Id;
    }

    public static PackageSnapshotEntity Create(long downloads, TrackedPackageEntity trackedPackage) =>
        new(0, DateTime.Now.Date, downloads, trackedPackage);

    public PackageSnapshotEntity ChangeDownloads(long downloads)
    {
        Downloads = downloads;
        TrackedPackage = null!;
        return this;
    }
}