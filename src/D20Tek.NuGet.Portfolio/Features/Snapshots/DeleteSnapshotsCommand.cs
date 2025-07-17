using D20Tek.NuGet.Portfolio.Persistence;

namespace D20Tek.NuGet.Portfolio.Features.Snapshots;

internal class DeleteSnapshotsCommand : AsyncCommand<DeleteSnapshotsCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandOption("-c|--collection-id")]
        [Description("The numeric id of the collection to delete download snapshots for.")]
        public int CollectionId { get; set; }

        [CommandOption("-d|--date")]
        [Description("Deletes a download snapshot for a partiular day.")]
        public DateOnly Date { get; set; }
    }

    private readonly IAnsiConsole _console;
    private readonly AppDbContext _dbContext;

    public DeleteSnapshotsCommand(IAnsiConsole console, AppDbContext dbContext) =>
        (_console, _dbContext) = (console, dbContext);

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        _console.CommandHeader().Render("Delete tracked package");
        return await settings.Pipe(s => EnsureIdInput(s))
                             .Pipe(s => _dbContext.Collections.GetEntityById(s.CollectionId)
                                 .BindAsync(c => _dbContext.DeleteSnapshotsByDate(c.Id, s.Date))
                                 .RenderAsync(_console, c => $"{c} snapshots deleted for: '{s.Date}'."));
    }

    private Settings EnsureIdInput(Identity<Settings> settings) =>
        settings.Iter(r => r.CollectionId = _console.AskIfDefault(r.CollectionId, "Enter the collection id:"))
                .Iter(r => r.Date = _console.PromptIfDefault(
                    r.Date,
                    "Enter snapshot date to delete:",
                    DateOnlyExtensions.Today()));
}
