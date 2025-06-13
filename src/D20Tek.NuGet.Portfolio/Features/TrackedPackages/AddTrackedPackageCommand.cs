using D20Tek.NuGet.Portfolio.Domain;
using D20Tek.NuGet.Portfolio.Persistence;

namespace D20Tek.NuGet.Portfolio.Features.TrackedPackages;

internal sealed class AddTrackedPackageCommand : AsyncCommand<AddTrackedPackageCommand.Request>
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

    private readonly IAnsiConsole _console;
    private readonly AppDbContext _dbContext;

    public AddTrackedPackageCommand(IAnsiConsole console, AppDbContext dbContext) =>
        (_console, _dbContext) = (console, dbContext);

    public override async Task<int> ExecuteAsync(CommandContext context, Request request)
    {
        _console.CommandHeader().Render("Add Tracked Package");
        return await request.Pipe(r => GetRequestInput(r))
                            .Pipe(r => Task.FromResult(TrackedPackageEntity.Create(r.PackageId, r.CollectionId)))
                            .BindAsync(SaveEntity)
                            .RenderAsync(_console, s => $"Created a new tracked package: '{s.PackageId}' [Id: {s.Id}].");
    }

    private Request GetRequestInput(Identity<Request> request) =>
        request.Iter(r => r.PackageId = _console.AskIfDefault(r.PackageId, "Enter the NuGet package id:"))
               .Iter(r => r.CollectionId = _console.AskIfDefault(r.CollectionId, "Add to collection with id:"));

    private async Task<Result<TrackedPackageEntity>> SaveEntity(TrackedPackageEntity entity) =>
        await TryAsync.RunAsync(async () =>
        {
            var result = _dbContext.TrackedPackages.Add(entity);
            await _dbContext.SaveChangesAsync();
            return Result<TrackedPackageEntity>.Success(result.Entity);
        });
}
