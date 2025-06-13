using D20Tek.NuGet.Portfolio.Domain;
using D20Tek.NuGet.Portfolio.Persistence;

namespace D20Tek.NuGet.Portfolio.Features.Collections;

internal sealed class EditCollectionCommand : AsyncCommand<EditCollectionCommand.Request>
{
    internal sealed class Request : CommandSettings
    {
        [CommandOption("-i|--id")]
        [Description("The id of the collection to edit.")]
        public int Id { get; set; }

        [CommandOption("-n|--name")]
        [Description("The collection's name property.")]
        public string Name { get; set; } = "";
    }

    private readonly IAnsiConsole _console;
    private readonly AppDbContext _dbContext;

    public EditCollectionCommand(IAnsiConsole console, AppDbContext dbContext) =>
        (_console, _dbContext) = (console, dbContext);

    public override async Task<int> ExecuteAsync(CommandContext context, Request request)
    {
        _console.CommandHeader().Render("Edit collection");
        return await GetEntity(request.Id)
            .Bind(entity => GetRequestInput(request, entity).Pipe(input => entity.Rename(input.Name)))
            .BindAsync(UpdateEntity)
            .RenderAsync(_console, s => $"Updated collection: '{s.Name}' [Id: {s.Id}].");
    }

    private Request GetRequestInput(Request request, CollectionEntity prevEntity) =>
        request.ToIdentity()
               .Iter(r => r.Name = _console.PromptIfDefault(r.Name, "Updated collection's name:", prevEntity.Name));

    private Result<CollectionEntity> GetEntity(int id) =>
        _console.AskIfDefault(id, "Id of collection to edit:")
                .Pipe(i => _dbContext.Collections.FirstOrDefault(c => c.Id == i)?
                           .Pipe(Result<CollectionEntity>.Success)
                               ?? Result<CollectionEntity>.Failure(Errors.EntityNotFound(nameof(CollectionEntity), i)));

    private async Task<Result<CollectionEntity>> UpdateEntity(CollectionEntity entity) =>
        await TryAsync.RunAsync(async () =>
        {
            await _dbContext.SaveChangesAsync();
            return Result<CollectionEntity>.Success(entity);
        });
}
