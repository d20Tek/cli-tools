using D20Tek.NuGet.Portfolio.Domain;
using D20Tek.NuGet.Portfolio.Features.PackageDownloads;
using D20Tek.NuGet.Portfolio.Persistence;

namespace D20Tek.NuGet.Portfolio.Features.PackageDownloads;

internal class GetDownloadsByCollectionIdCommand : AsyncCommand<GetDownloadsByCollectionIdCommand.CollectionId>
{
    public sealed class CollectionId : CommandSettings
    {
        [CommandOption("-c|--collection-id")]
        [Description("The numeric id of the collection of tracked packages to retrieve.")]
        public int Value { get; set; }
    }

    private readonly IAnsiConsole _console;
    private readonly AppDbContext _dbContext;
    private readonly INuGetSearchClient _client;

    public GetDownloadsByCollectionIdCommand(
        IAnsiConsole console,
        AppDbContext dbContext,
        INuGetSearchClient client) =>
        (_console, _dbContext, _client) = (console, dbContext, client);

    public override async Task<int> ExecuteAsync(CommandContext context, CollectionId id)
    {
        _console.CommandHeader().Render("Get collection package downloads");
        return await id.Pipe(i => EnsureIdInput(i))
                       .Pipe(i => EnsureCollectionExists(i)
                           .Map(_ => GetTrackedPackages(i)))
                       .BindAsync(p => _client.RetrieveDownloadSnapshots(p))
                       .IterAsync(s => _console.RenderDownloadSnapshots(s))
                       .RenderAsync(_console, s => $"Retrieved downloads for {s.Length} tracked packages.");
    }

    private CollectionId EnsureIdInput(Identity<CollectionId> id) =>
        id.Iter(r => r.Value = _console.AskIfDefault(r.Value, "Enter the collection id:"));

    private Result<CollectionEntity> EnsureCollectionExists(CollectionId id) =>
        _dbContext.Collections.FirstOrDefault(c => c.Id == id.Value)?
            .Pipe(Result<CollectionEntity>.Success)
                ?? Result<CollectionEntity>.Failure(Errors.EntityNotFound(nameof(CollectionEntity), id.Value));

    public TrackedPackageEntity[] GetTrackedPackages(CollectionId id) =>
        [.. _dbContext.TrackedPackages.Where(x => x.CollectionId == id.Value)];
}
