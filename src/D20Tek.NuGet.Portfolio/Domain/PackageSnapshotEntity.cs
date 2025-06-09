namespace D20Tek.NuGet.Portfolio.Domain;

public sealed class PackageSnapshotEntity
{
    public int Id { get; set; }

    public DateTime SnapshotDate { get; set; }
    
    public int Downloads { get; set; }

    public int TrackedPackageId { get; set; }
    
    public TrackedPackageEntity TrackedPackage { get; set; } = null!;
}