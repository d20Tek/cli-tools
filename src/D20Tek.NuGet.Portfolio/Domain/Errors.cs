namespace D20Tek.NuGet.Portfolio.Domain;

internal static class Errors
{
    public static readonly Error CollectionNameRequired = 
        Error.Validation("Collection.Name.Required", "The collection name is required.");

    public static readonly Error CollectionNameMaxLength =
        Error.Validation("Collection.Name.MaxLength", "The collection name must be 64 characters or less.");

    public static Error EntityNotFound(string entityType, int id) =>
        Error.NotFound($"{entityType}.NotFound", $"The entity with id={id} was not found.");
}
