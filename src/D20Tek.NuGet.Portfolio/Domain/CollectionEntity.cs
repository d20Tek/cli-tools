namespace D20Tek.NuGet.Portfolio.Domain;

public sealed class CollectionEntity
{
    public const int NameMaxLength = 64;

    public int Id { get; private set; }

    public string Name { get; private set; }

    public List<TrackedPackageEntity> TrackedPackages { get; set; } = [];

    private CollectionEntity(int id, string name)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(id);
        ArgumentNullException.ThrowIfNullOrEmpty(name);
        Id = id; 
        Name = name;
    }

    public static Result<CollectionEntity> Create(string name) =>
        Validate(name).Map(() => new CollectionEntity(0, name));

    private static ValidationErrors Validate(string name) =>
        ValidationErrors.Create()
                        .AddIfError(() => string.IsNullOrEmpty(name), Errors.CollectionNameRequired)
                        .AddIfError(() => name.Length > NameMaxLength, Errors.CollectionNameMaxLength);

    public void Rename(string name)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(name);
        Name = name;
    }
}
