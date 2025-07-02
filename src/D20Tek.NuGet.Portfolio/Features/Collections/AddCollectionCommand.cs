using D20Tek.NuGet.Portfolio.Domain;
using D20Tek.NuGet.Portfolio.Persistence;

namespace D20Tek.NuGet.Portfolio.Features.Collections;

internal sealed class AddCollectionCommand : AsyncCommand<AddCollectionCommand.Request>
{
    internal sealed class Request : CommandSettings
    {
        [CommandOption("-n|--name")]
        [Description("The collection's name property.")]
        public string Name { get; set; } = "";
    }

    private readonly IAnsiConsole _console;
    private readonly AppDbContext _dbContext;

    public AddCollectionCommand(IAnsiConsole console, AppDbContext dbContext) =>
        (_console, _dbContext) = (console, dbContext);

    public override async Task<int> ExecuteAsync(CommandContext context, Request request)
    {
        _console.CommandHeader().Render("Add new collection");
        return await request.Pipe(GetRequestInput)
                            .Pipe(r => Task.FromResult(CollectionEntity.Create(r.Name)))
                            .BindAsync(entity => _dbContext.Collections.CreateEntity(entity, _dbContext))
                            .RenderAsync(_console, s => $"Created a new collection: '{s.Name}' [Id: {s.Id}].");
    }

    private Request GetRequestInput(Request request) =>
        request.ToIdentity().Iter(r => r.Name = _console.AskIfDefault(r.Name, "Enter the new collection's name:"));
}
