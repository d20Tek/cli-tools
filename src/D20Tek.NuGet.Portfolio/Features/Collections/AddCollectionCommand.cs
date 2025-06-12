using D20Tek.NuGet.Portfolio.Common;
using D20Tek.NuGet.Portfolio.Common.Controls;
using D20Tek.NuGet.Portfolio.Domain;
using D20Tek.NuGet.Portfolio.Persistence;

namespace D20Tek.NuGet.Portfolio.Features.Collections;

internal sealed class AddCollectionCommand : AsyncCommand<AddCollectionCommand.Request>
{
    private readonly IAnsiConsole _console;
    private readonly AppDbContext _dbContext;

    public AddCollectionCommand(IAnsiConsole console, AppDbContext dbContext) =>
        (_console, _dbContext) = (console, dbContext);

    internal class Request : CommandSettings
    {
        [CommandOption("-n|--name")]
        [Description("The collection's name property.")]
        public string Name { get; set; } = "";
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Request request)
    {
        _console.CommandHeader().Render("Add new collection");
        return await request.Pipe(GetRequestInput)
                            .Pipe(r => Task.FromResult(CollectionEntity.Create(r.Name)))
                            .BindAsync(SaveEntity)
                            .RenderAsync(_console, s => $"Created a new collection: '{s.Name}' [Id: {s.Id}].");
    }

    private Request GetRequestInput(Request request) =>
        request.ToIdentity().Iter(r => r.Name = _console.AskIfDefault(r.Name, "Enter the new collection's name:"));

    private async Task<Result<CollectionEntity>> SaveEntity(CollectionEntity entity) =>
        await TryAsync.RunAsync(async () =>
        {
            var result = _dbContext.Collections.Add(entity);
            await _dbContext.SaveChangesAsync();
            return Result<CollectionEntity>.Success(result.Entity);
        });
}
