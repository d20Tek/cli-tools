using D20Tek.NuGet.Portfolio.Domain;
using D20Tek.NuGet.Portfolio.Persistence;

namespace D20Tek.NuGet.Portfolio.Features.TrackedPackages;

internal sealed class AddTrackedPackageCommand(IAnsiConsole console, AppDbContext dbContext) :
    AsyncCommand<AddTrackedPackageCommand.Request>
{
    public sealed class Request : CommandSettings
    {
        [CommandOption("-p|--package-id")]
        [Description("The NuGet package id that you wish to track.")]
        public string PackageId { get; set; } = "";

        [CommandOption("-c|--collection-id")]
        [Description("The id of the collection to add the package to.")]
        public int CollectionId { get; set; }
    }

    private readonly IAnsiConsole _console = console;
    private readonly AppDbContext _dbContext = dbContext;

    public override async Task<int> ExecuteAsync(CommandContext context, Request request, CancellationToken token)
    {
        _console.CommandHeader().Render("Add Tracked Package");
        return await request.Pipe(r => GetRequestInput(r))
                            .Pipe(r => Task.FromResult(TrackedPackageEntity.Create(r.PackageId, r.CollectionId)))
                            .BindAsync(SaveEntity)
                            .RenderAsync(_console, s => $"Created a new tracked package: '{s.PackageId}' [Id: {s.Id}].");
    }

    private Request GetRequestInput(Identity<Request> request) =>
        request.Iter(r =>
                    r.PackageId = _console.AskIfDefault(r.PackageId, "Enter the NuGet package id:", Globals.AppPrompt))
               .Iter(r => 
                    r.CollectionId = _console.AskIfDefault(r.CollectionId, "Add to collection with id:", Globals.AppPrompt));

    private async Task<Result<TrackedPackageEntity>> SaveEntity(TrackedPackageEntity entity) =>
        await _dbContext.Collections.GetEntityById(entity.CollectionId)
                                    .BindAsync(coll => _dbContext.TrackedPackages.CreateEntity(entity, _dbContext));
}
