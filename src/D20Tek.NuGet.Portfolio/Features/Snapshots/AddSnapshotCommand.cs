using D20Tek.NuGet.Portfolio.Abstractions;
using D20Tek.NuGet.Portfolio.Domain;
using D20Tek.NuGet.Portfolio.Features.PackageDownloads;
using D20Tek.NuGet.Portfolio.Persistence;
using Microsoft.EntityFrameworkCore;

namespace D20Tek.NuGet.Portfolio.Features.Snapshots;

internal sealed class AddSnapshotCommand : AsyncCommand<AddSnapshotCommand.CollectionId>
{
    public sealed class CollectionId : CommandSettings
    {
        [CommandOption("-c|--collection-id")]
        [Description("The numeric id of the collection to get download snapshots for.")]
        public int Value { get; set; }
    }

    private readonly IAnsiConsole _console;
    private readonly AppDbContext _dbContext;
    private readonly INuGetSearchClient _client;

    public AddSnapshotCommand(IAnsiConsole console, AppDbContext dbContext, INuGetSearchClient client) =>
        (_console, _dbContext, _client) = (console, dbContext, client);

    public override async Task<int> ExecuteAsync(CommandContext context, CollectionId id)
    {
        _console.CommandHeader().Render("Add snapshot package downloads");
        return await id.Pipe(i => EnsureIdInput(i))
                       .Pipe(i => _dbContext.Collections.GetEntityById(i.Value)
                           .Map(_ => _dbContext.GetTrackPackagesByCollectionIdAsTracking(i.Value)))
                       .BindAsync(p => _client.RetrieveDownloadSnapshots(p))
                       .IterAsync(s => _console.RenderDownloadSnapshots(s))
                       .BindAsync(s => Upsert(s))
                       .RenderAsync(_console, s => $"Snapshot downloads saved for {s.Length} tracked packages.");
    }

    private CollectionId EnsureIdInput(Identity<CollectionId> id) =>
        id.Iter(r => r.Value = _console.AskIfDefault(r.Value, "Enter the collection id:"));

    private async Task<Result<PackageSnapshotEntity[]>> Upsert(PackageSnapshotEntity[] snapshots)
    {
        try
        {
            foreach (var snapshot in snapshots)
            {
                (await _dbContext.PackageSnapshots.FirstOrDefaultAsync(x => x.SnapshotDate == snapshot.SnapshotDate))
                                 .ToOption()
                                 .Match(
                                    s => s.ChangeDownloads(snapshot.Downloads),
                                    () => _dbContext.PackageSnapshots.Add(snapshot).Entity);
            }

            await _dbContext.SaveChangesAsync();
            return snapshots;
        }
        catch (Exception ex)
        {
            return Result<PackageSnapshotEntity[]>.Failure(ex);
        }
    }
}
