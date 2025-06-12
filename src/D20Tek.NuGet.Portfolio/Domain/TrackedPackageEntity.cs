namespace D20Tek.NuGet.Portfolio.Domain;

public sealed class TrackedPackageEntity
{
    public int Id { get; private set; }

    public string PackageId { get; private set; } = string.Empty;

    public DateOnly DateAdded { get; private set; }

    public int CollectionId { get; private set; }

    // EF navigation property (required by EF)
    public CollectionEntity Collection { get; private set; } = null!;

    // Navigation collection property (must have setter or be initialized)
    public List<PackageSnapshotEntity> Snapshots { get; private set; } = [];

    private TrackedPackageEntity()
    {
    }

    private TrackedPackageEntity(string packageId, DateOnly dateAdded, int collectionId)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(packageId, nameof(packageId));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(collectionId, nameof(collectionId));

        PackageId = packageId;
        DateAdded = dateAdded;
        CollectionId = collectionId;
    }

    public static TrackedPackageEntity Create(string packageId, int collectionId) =>
        new(packageId, DateOnly.FromDateTime(DateTimeOffset.Now.LocalDateTime), collectionId);
}
