using D20Tek.Tools.DevLog.Contracts;
using D20Tek.Tools.DevLog.Services;

namespace D20Tek.Tools.DevLog.Commands;

internal sealed class ViewLogCommand(IDevLogService service, IAnsiConsole console)
    : Command<ViewLogCommand.Settings>
{
    private readonly IDevLogService _service = service;
    private readonly IAnsiConsole _console = console;

    public sealed class Settings : CommandSettings
    {
        [CommandOption("-d|--date")]
        [Description("The date within the week to view (defaults to current week). Format: MM-dd-yyyy.")]
        public string Date { get; init; } = string.Empty;

        [CommandOption("-f|--folder")]
        [Description("The folder path where dev-log files are stored.")]
        public string Folder { get; init; } = Constants.DefaultLogFolder;
    }

    public override int Execute(CommandContext context, Settings settings, CancellationToken _)
    {
        var date = ParseDate(settings.Date);
        var weekStart = DevLogEntry.GetWeekStart(date ?? DateOnly.FromDateTime(DateTime.Today));

        return _service.ViewLog(settings.Folder, date)
            .Iter(_ => _console.MarkupLines(Constants.ViewLogTitle(weekStart), Constants.Separator))
            .Iter(content => _console.Write(new Text(content + "\n")))
            .Iter(_ => _console.WriteLine())
            .Render(_console, _ => Constants.ViewLogSucceeded);
    }

    private static DateOnly? ParseDate(string dateString) =>
        DateOnly.TryParse(dateString, out var date) ? date : null;
}
