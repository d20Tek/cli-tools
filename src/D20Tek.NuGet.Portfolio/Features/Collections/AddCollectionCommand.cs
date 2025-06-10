using D20Tek.Functional.Async;
using D20Tek.NuGet.Portfolio.Common;
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
                            .Pipe(Validate)
                            .Map(CreateEntity)
                            .MatchAsync(
                                s => { _console.MarkupLine($"[green]Success![/] Created a new collection: '{request.Name}'"); return Task.FromResult(Globals.S_OK); },
                                e => { _console.MarkupLine($"[red]Error:[/] {e.First().Message}"); return Task.FromResult(Globals.E_FAIL); });
    }

    private Request GetRequestInput(Request request)
    {
        if (string.IsNullOrEmpty(request.Name))
        {
            _console.WriteLine("Enter the new collection's name:");
            request.Name = _console.Ask<string>(Globals.AppPrompt);
        }

        return request;
    }

    private Result<Request> Validate(Request request) =>
        ValidationErrors.Create()
                        .AddIfError(() => string.IsNullOrEmpty(request.Name), Error.Validation("Collection.Name.Required", "The collection name is required."))
                        .AddIfError(() => request.Name.Length > 64, Error.Validation("Collection.Name.MaxLength", "The collection name must be 64 characters or less."))
                        .Map(() => request);

    private async Task<Result<CollectionEntity>> CreateEntity(Request request)
    {
        try
        {
            var entity = CollectionEntity.Create(request.Name);
            var result = _dbContext.Collections.Add(entity);
            await _dbContext.SaveChangesAsync();
            return Result<CollectionEntity>.Success(result.Entity);
        }
        catch (Exception ex)
        {
            return Result<CollectionEntity>.Failure(ex);
        }
    }
}
