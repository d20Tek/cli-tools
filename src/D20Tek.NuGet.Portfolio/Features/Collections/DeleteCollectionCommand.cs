using D20Tek.NuGet.Portfolio.Common;
using D20Tek.NuGet.Portfolio.Common.Controls;
using D20Tek.NuGet.Portfolio.Domain;
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

    public override Task<int> ExecuteAsync(CommandContext context, CollectionId id)
    {
        _console.CommandHeader().Render("Delete collection");
        return id.Pipe(i => EnsureIdInput(i))
                 .Pipe(i => DeleteEntity(i))
                 .RenderAsync(_console, s => $"Collection deleted: '{s.Name}' [Id: {s.Id}].");
    }

    private CollectionId EnsureIdInput(Identity<CollectionId> id) =>
        id.Iter(r => r.Value = _console.AskIfDefault(r.Value, "Enter the collection id:"));

    private async Task<Result<CollectionEntity>> DeleteEntity(CollectionId id) =>
        await TryAsync.RunAsync(() =>
            GetEntity(id).BindAsync(async entity =>
            {
                _dbContext.Collections.Remove(entity);
                await _dbContext.SaveChangesAsync();
                return Result<CollectionEntity>.Success(entity);
            }));

    private Result<CollectionEntity> GetEntity(CollectionId id) =>
        _dbContext.Collections.FirstOrDefault(c => c.Id == id.Value)?
            .Pipe(Result<CollectionEntity>.Success)
                ?? Result<CollectionEntity>.Failure(Errors.EntityNotFound(nameof(CollectionEntity), id.Value));
}
