using D20Tek.NuGet.Portfolio.Persistence;

namespace D20Tek.NuGet.Portfolio.Features.Collections;

internal sealed class DeleteCollectionCommand : AsyncCommand<DeleteCollectionCommand.CollectionId>
{
    public sealed class CollectionId : CommandSettings
    {
        [CommandOption("-i|--id")]
        [Description("The id of the collection to delete.")]
        public int Value { get; set; }
    }

    private readonly IAnsiConsole _console;
    private readonly AppDbContext _dbContext;

    public DeleteCollectionCommand(IAnsiConsole console, AppDbContext dbContext) =>
        (_console, _dbContext) = (console, dbContext);

    public override async Task<int> ExecuteAsync(CommandContext context, CollectionId id)
    {
        _console.CommandHeader().Render("Delete collection");
        return await id.Pipe(i => EnsureIdInput(i))
                       .Pipe(i => _dbContext.Collections.DeleteEntityById(i.Value, _dbContext))
                       .RenderAsync(_console, s => $"Collection deleted: '{s.Name}' [Id: {s.Id}].");
    }

    private CollectionId EnsureIdInput(Identity<CollectionId> id) =>
        id.Iter(r => r.Value = _console.AskIfDefault(r.Value, "Enter the collection id:"));
}
