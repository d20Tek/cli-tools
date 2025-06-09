namespace D20Tek.NuGet.Portfolio.Domain;

public sealed class TrackedPackageEntity
{
    public int Id { get; set; }

    public string PackageId { get; set; } = string.Empty;

    public DateTime DateAdded { get; set; }

    public int CollectionId { get; set; }

    public CollectionEntity Collection { get; set; } = null!;

    public List<PackageSnapshotEntity> Snapshots { get; set; } = new();
}
