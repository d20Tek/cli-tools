using System.Globalization;
using D20Tek.Tools.DevLog.Services;

namespace D20Tek.Tools.DevLog.Commands;

internal sealed class ListLogsCommand(IDevLogService service, IAnsiConsole console) : Command<ListLogsCommand.Settings>
{
    private readonly IDevLogService _service = service;
    private readonly IAnsiConsole _console = console;

    public sealed class Settings : FolderSettings { }

    protected override int Execute(CommandContext context, Settings settings, CancellationToken _) =>
        _service.ListLogs(settings.Folder)
            .Iter(_ => _console.MarkupLine(Constants.ListLogsTitle))
            .Iter(files => DisplayFiles(files, settings.Folder))
            .Render(_console, _ => Constants.ListLogsSucceeded);

    private void DisplayFiles(IEnumerable<string> files, string folder)
    {
        var fileList = files.OrderByDescending(f => f).ToList();
        if (fileList.Count == 0)
        {
            _console.MarkupLine(Constants.ListLogsEmpty(folder));
        }
        else
        {
            fileList.ForEach(f => _console.MarkupLine(FormatFileEntry(f)));
            _console.WriteLine();
        }
    }

    private static string FormatFileEntry(string filePath)
    {
        var fileName = Path.GetFileName(filePath);
        var date = ParseDateFromFileName(filePath);
        return date.HasValue
            ? $" [yellow]{fileName}[/]  (Week of [green]{date.Value.ToString(Constants.DateDisplayFormat)}[/])"
            : $" [yellow]{fileName}[/]";
    }

    private static DateOnly? ParseDateFromFileName(string filePath)
    {
        var name = Path.GetFileNameWithoutExtension(Path.GetFileName(filePath));
        return (name.Length >= 8 && DateOnly.TryParseExact(
                name[^8..], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            ? date : null;
    }
}
