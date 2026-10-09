using D20Tek.Spectre.Console.Extensions.Controls;
using D20Tek.Tools.DevLog.Contracts;
using D20Tek.Tools.DevLog.Services;

namespace D20Tek.Tools.DevLog.Commands;

internal sealed class EditEntryCommand(IDevLogService service, IAnsiConsole console)
    : Command<EditEntryCommand.Settings>
{
    private readonly IDevLogService _service = service;
    private readonly IAnsiConsole _console = console;

    public sealed class Settings : EntrySettings
    {
        [CommandArgument(0, "<PROJECT>")]
        [Description("The name of the project entry to edit.")]
        public string ProjectName { get; init; } = string.Empty;
    }

    public override int Execute(CommandContext context, Settings settings, CancellationToken _)
    {
        var date = settings.Date.ParseDate();
        var weekStart = DevLogEntry.GetWeekStart(date ?? DateOnly.FromDateTime(DateTime.Today));

        return ProjectNameValidator.Validate(settings.ProjectName)
            .Iter(_ => _console.MarkupLine(Constants.EditEntryTitle(settings.ProjectName, weekStart)))
            .Map(_ => LoadAccomplishments(settings.Folder, settings.ProjectName, date))
            .Iter(DisplayNumberedAccomplishments)
            .Map(EditAccomplishments)
            .Bind(accomplishments => _service.EditEntry(settings.Folder, settings.ProjectName, accomplishments, date))
            .Render(_console, _ => Constants.EditEntrySuccess);
    }

    private List<string> LoadAccomplishments(string folder, string projectName, DateOnly? date) =>
        _service.GetAccomplishments(folder, projectName, date)
                .Match(acc => acc, e => []);

    private void DisplayNumberedAccomplishments(List<string> items)
    {
        _console.MarkupLine(Constants.CurrentAccomplishmentsLabel);
        if (items.Count == 0) _console.MarkupLine(Constants.EmptyList);

        for (var i = 0; i < items.Count; i++)
        {
            _console.MarkupLine($" [[{i + 1}]] {Markup.Escape(items[i])}");
        }
    }

    private List<string> EditAccomplishments(List<string> items)
    {
        var prompt = new EditablePrompt(Constants.EditLineNewTextPrompt);
        var edited = items.ToList();
        while (GetAccomplishmentInput(out var input))
        {
            if (!int.TryParse(input, out var lineNum) || lineNum < 1 || lineNum > edited.Count)
            {
                _console.MarkupLine(Constants.InvalidLineNumber(edited.Count));
                continue;
            }

            var index = lineNum - 1;
            var currentText = edited[index];
            _console.WriteMessages($"{Constants.EditLineCurrentTextPrompt}{currentText}");

            edited[index] = _console.Prompt(prompt).Trim();
        }

        return edited;
    }

    private bool GetAccomplishmentInput(out string input) => !string.IsNullOrWhiteSpace(
        input = _console.Prompt(new TextPrompt<string>(Constants.EditLinePrompt).AllowEmpty()));
}

