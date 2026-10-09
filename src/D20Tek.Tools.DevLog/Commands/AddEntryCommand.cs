using D20Tek.Tools.DevLog.Contracts;
using D20Tek.Tools.DevLog.Services;

namespace D20Tek.Tools.DevLog.Commands;

internal sealed class AddEntryCommand(IDevLogService service, IAnsiConsole console) : Command<AddEntryCommand.Settings>
{
    private readonly IDevLogService _service = service;
    private readonly IAnsiConsole _console = console;

    public sealed class Settings : EntrySettings
    {
        [CommandArgument(0, "<PROJECT>")]
        [Description("The name of the project to add the dev-log entry for.")]
        public string ProjectName { get; init; } = string.Empty;
    }

    public override int Execute(CommandContext context, Settings settings, CancellationToken _)
    {
        var date = settings.Date.ParseDate();
        var weekStart = DevLogEntry.GetWeekStart(date ?? DateOnly.FromDateTime(DateTime.Today));

        return ProjectNameValidator.Validate(settings.ProjectName)
            .Iter(_ => _console.MarkupLine(Constants.AddEntryTitle(settings.ProjectName, weekStart)))
            .Map(_ => PromptForAccomplishments())
            .Bind(accomplishments => _service.AddEntry(settings.Folder, settings.ProjectName, accomplishments, date))
            .Render(_console, _ => Constants.AddEntrySuccess);
    }

    private List<string> PromptForAccomplishments()
    {
        _console.MarkupLine(Constants.AccomplishmentsPrompt);
        var accomplishments = new List<string>();
        var prompt = new EditablePrompt(Constants.AccomplishmentItemPrompt);
        string input;

        while (!string.IsNullOrWhiteSpace(input = _console.Prompt(prompt)))
        {
            accomplishments.Add(input.Trim());
        }

        return accomplishments;
    }
}
