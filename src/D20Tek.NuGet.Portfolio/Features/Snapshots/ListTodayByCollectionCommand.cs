using D20Tek.NuGet.Portfolio.Domain;
using D20Tek.NuGet.Portfolio.Features.Helpers;
using D20Tek.NuGet.Portfolio.Persistence;

namespace D20Tek.NuGet.Portfolio.Features.Snapshots;

internal sealed class ListTodayByCollectionCommand(IAnsiConsole console, AppDbContext dbContext) :
    Command<CollectionId>
{
    private readonly IAnsiConsole _console = console;
    private readonly AppDbContext _dbContext = dbContext;

    public override int Execute(CommandContext context, CollectionId id, CancellationToken token)
    {
        _console.CommandHeader().Render("Packages snapshots");
        return id.Pipe(i => EnsureIdInput(i))
                 .Pipe(i => _dbContext.Collections.GetEntityById(i.Value)
                     .Bind(_ => _dbContext.GetSnapshotsForCollection(i.Value, DateOnlyExtensions.Today())))
                 .Iter(s => RenderSnapshotsForDate(s))
                 .Render(_console, s => $"Retrieved package snapshots for {s.Length} tracked packages.");
    }

    private CollectionId EnsureIdInput(Identity<CollectionId> id) =>
        id.Iter(r => r.Value = _console.AskIfDefault(r.Value, "Enter the collection id:"));

    public void RenderSnapshotsForDate(PackageSnapshotEntity[] snapshots) =>
        _console.RenderTableWithTitle(
            "Package downloads (today)",
            DownloadsTableBuilder.Create()
                                 .WithHeader()
                                 .WithRows(snapshots)
                                 .WithTotals(snapshots)
                                 .Build());
}
