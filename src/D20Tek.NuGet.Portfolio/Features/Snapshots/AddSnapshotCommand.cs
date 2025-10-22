using D20Tek.NuGet.Portfolio.Abstractions;
using D20Tek.NuGet.Portfolio.Features.Helpers;
using D20Tek.NuGet.Portfolio.Persistence;

namespace D20Tek.NuGet.Portfolio.Features.Snapshots;

internal sealed class AddSnapshotCommand(IAnsiConsole console, AppDbContext dbContext, INuGetSearchClient client) :
    AsyncCommand<CollectionId>
{
    private readonly IAnsiConsole _console = console;
    private readonly AppDbContext _dbContext = dbContext;
    private readonly INuGetSearchClient _client = client;

    public override async Task<int> ExecuteAsync(CommandContext context, CollectionId id)
    {
        _console.CommandHeader().Render("Add snapshot package downloads");
        return await id.Pipe(i => EnsureIdInput(i))
                       .Pipe(i => _dbContext.Collections.GetEntityById(i.Value)
                           .Map(_ => _dbContext.GetTrackPackagesByCollectionIdAsTracking(i.Value)))
                       .BindAsync(p => _client.RetrieveDownloadSnapshots(p))
                       .IterAsync(s => _console.RenderDownloadSnapshots(s))
                       .BindAsync(s => _dbContext.UpsertSnapshots(s))
                       .RenderAsync(_console, s => $"Snapshot downloads saved for {s.Length} tracked packages.");
    }

    private CollectionId EnsureIdInput(Identity<CollectionId> id) =>
        id.Iter(r => r.Value = _console.AskIfDefault(r.Value, "Enter the collection id:"));
}
