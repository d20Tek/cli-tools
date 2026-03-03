using D20Tek.Tools.DevLog.Contracts;
using D20Tek.Tools.DevLog.Services;

namespace D20Tek.Tools.DevLog.Commands;

internal sealed class AddEntryCommand(IDevLogService service, IAnsiConsole console)
    : Command<AddEntryCommand.Settings>
{
    private readonly IDevLogService _service = service;
    private readonly IAnsiConsole _console = console;

    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<PROJECT>")]
        [Description("The name of the project to add the dev-log entry for.")]
        public string ProjectName { get; init; } = string.Empty;

        [CommandOption("-d|--date")]
        [Description("The date for the dev-log entry (defaults to today). Format: MM-dd-yyyy.")]
        public string Date { get; init; } = string.Empty;

        [CommandOption("-f|--folder")]
        [Description("The folder path where dev-log files are stored.")]
        public string Folder { get; init; } = Constants.DefaultLogFolder;
    }

    public override int Execute(CommandContext context, Settings settings, CancellationToken _)
    {
        var date = ParseDate(settings.Date);
        var weekStart = DevLogEntry.GetWeekStart(date ?? DateOnly.FromDateTime(DateTime.Today));

        return ValidateProjectName(settings.ProjectName)
            .Iter(_ => _console.MarkupLine(Constants.AddEntryTitle(settings.ProjectName, weekStart)))
            .Map(_ => PromptForAccomplishments())
            .Bind(accomplishments => _service.AddEntry(settings.Folder, settings.ProjectName, accomplishments, date))
            .Render(_console, _ => Constants.AddEntrySuccess);
    }

    private List<string> PromptForAccomplishments()
    {
        _console.MarkupLine(Constants.AccomplishmentsPrompt);
        var accomplishments = new List<string>();
        string input;
        while (!string.IsNullOrWhiteSpace(
            input = _console.Prompt(new TextPrompt<string>(Constants.AccomplishmentItemPrompt).AllowEmpty())))
        {
            accomplishments.Add(input.Trim());
        }

        return accomplishments;
    }

    private static DateOnly? ParseDate(string dateString) => DateOnly.TryParse(dateString, out var date) ? date : null;

    private static Result<string> ValidateProjectName(string projectName) =>
        string.IsNullOrWhiteSpace(projectName)
            ? Result<string>.Failure(Constants.Errors.ProjectNameRequired)
            : Result<string>.Success(projectName);
}
