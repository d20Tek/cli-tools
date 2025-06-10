namespace D20Tek.NuGet.Portfolio.Domain;

public sealed class CollectionEntity
{
    public int Id { get; }

    public string Name { get; private set; }

    public List<TrackedPackageEntity> TrackedPackages { get; set; } = [];

    private CollectionEntity(int id, string name)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(id);
        ArgumentNullException.ThrowIfNullOrEmpty(name);
        Id = id; 
        Name = name;
    }

    public static CollectionEntity Create(string name) => new(0, name);

    public void Rename(string name)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(name);
        Name = name;
    }
}
