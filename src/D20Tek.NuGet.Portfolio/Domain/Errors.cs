namespace D20Tek.NuGet.Portfolio.Domain;

internal static class Errors
{
    public static Error EntityNotFound(string entityType, int id) =>
        Error.NotFound($"{entityType}.NotFound", $"The {entityType} with id={id} was not found.");

    public static readonly Error CollectionNameRequired =
        Error.Validation("Collection.Name.Required", "The collection name is required.");

    public static readonly Error CollectionNameMaxLength =
        Error.Validation("Collection.Name.MaxLength", "The collection name must be 64 characters or less.");

    public static readonly Error PackageIdRequired =
        Error.Validation("TrackedPackage.PackageId.Required", "The tracked package id is required.");

    public static readonly Error PackageIdMaxLength =
        Error.Validation("TrackedPackage.PackageId.MaxLength", "The tracked package id must be 256 characters or less.");

    public static readonly Error CollectionIdRequired =
        Error.Validation("TrackedPackage.CollectionId.Required", "The tracked package's collection id is required.");
}
