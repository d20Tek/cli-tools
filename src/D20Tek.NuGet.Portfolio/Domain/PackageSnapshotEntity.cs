namespace D20Tek.NuGet.Portfolio.Domain;

public sealed class PackageSnapshotEntity
{
    public int Id { get; private set; }

    public DateTime SnapshotDate { get; private set; }
    
    public int Downloads { get; private set; }

    public int TrackedPackageId { get; private set; }
    
    public TrackedPackageEntity TrackedPackage { get; private set; } = null!;

    private PackageSnapshotEntity() { }

    private PackageSnapshotEntity(int id, DateTime snapshotDate, int downloads, TrackedPackageEntity trackedPackage)
    {
        Id = id;
        SnapshotDate = snapshotDate;
        Downloads = downloads;
        TrackedPackage = trackedPackage;
        TrackedPackageId = trackedPackage.Id;
    }

    public static PackageSnapshotEntity Create(int downloads, TrackedPackageEntity trackedPackage) =>
        new(0, DateTime.Now, downloads, trackedPackage);
}