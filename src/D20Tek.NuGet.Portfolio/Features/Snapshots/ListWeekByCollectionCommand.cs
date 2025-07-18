using D20Tek.NuGet.Portfolio.Domain;
using D20Tek.NuGet.Portfolio.Features.Helpers;
using D20Tek.NuGet.Portfolio.Persistence;

namespace D20Tek.NuGet.Portfolio.Features.Snapshots;

internal sealed class ListWeekByCollectionCommand : Command<CollectionId>
{
    private readonly IAnsiConsole _console;
    private readonly AppDbContext _dbContext;

    public ListWeekByCollectionCommand(IAnsiConsole console, AppDbContext dbContext) =>
        (_console, _dbContext) = (console, dbContext);

    public override int Execute(CommandContext context, CollectionId id)
    {
        _console.CommandHeader().Render("Packages snapshots");
        return id.Pipe(i => EnsureIdInput(i))
                 .Pipe(i => _dbContext.Collections.GetEntityById(i.Value)
                     .Bind(_ => _dbContext.GetSnapshotsForCollection(i.Value, DateRange.ForWeekEnding())))
                 .Iter(s => RenderSnapshotsForWeek(s))
                 .Render(_console, s => $"Retrieved package snapshots for the last week.");
    }

    private CollectionId EnsureIdInput(Identity<CollectionId> id) =>
        id.Iter(r => r.Value = _console.AskIfDefault(r.Value, "Enter the collection id:"));

    public void RenderSnapshotsForWeek(PackageSnapshotEntity[] snapshots) =>
        _console.RenderTableWithTitle(
            "Package downloads (week)",
            DownloadsTableBuilder.Create()
                                 .WithHeader()
                                 .WithRows(snapshots)
                                 .WithTotals(snapshots)
                                 .Build());
}
