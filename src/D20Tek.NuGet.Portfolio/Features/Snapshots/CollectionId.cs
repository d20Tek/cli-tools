namespace D20Tek.NuGet.Portfolio.Features.Snapshots;

public sealed class CollectionId : CommandSettings
{
    [CommandOption("-c|--collection-id")]
    [Description("The numeric id of the collection to use in this command.")]
    public int Value { get; set; }
}
