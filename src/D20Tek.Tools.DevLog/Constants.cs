namespace D20Tek.Tools.DevLog;

internal static class Constants
{
    public const string AppName = "dev-log";
    public const string AppInitializeSuccessMsg = "Log the work you've performed on your projects.";
    public const string AppGetStartedMsg = "[green]Running interactive mode.[/] Type 'exit' to quit or '--help' to see available commands.";
    public const string AppPrompt = "log>";
    public const string AppExitMessage = "[green]Bye![/] Thanks for managing you dev-log.";

    public const string MarkdownExtension = ".md";
    public const string MarkdownSearchPattern = "*.md";
    public const string DefaultLogFolder = ".";
    public const string FileNameFormat = "dev-log-{0:yyyyMMdd}.md";
    public const string DateDisplayFormat = "MMMM d, yyyy";
    public const string AccomplishmentBullet = "- ";
    public const string AccomplishmentsPrompt = "Enter accomplishments (one per line, empty line to finish):";
    public const string AccomplishmentItemPrompt = "  > ";
    public const string CurrentAccomplishmentsLabel = "Current accomplishments:";
    public const string EditLinePrompt = "Enter line number to edit (empty to finish):";
    public const string EditLineNewTextPrompt = "  New text:";

    public static string WeekHeader(DateOnly weekStart) => $"## Week of {weekStart.ToString(DateDisplayFormat)}";

    public static string ProjectHeader(string projectName) => $"### {projectName}";

    public static string AddEntryTitle(string project, DateOnly weekStart) =>
        $"[purple]Adding entry[/] for [yellow]'{project}'[/] to week of [yellow]{weekStart.ToString(DateDisplayFormat)}[/]";

    public static string ViewLogTitle(DateOnly weekStart) =>
        $"[purple]Dev-log[/] for week of [yellow]{weekStart.ToString(DateDisplayFormat)}[/]";

    public static string EditEntryTitle(string project, DateOnly weekStart) =>
        $"[purple]Editing entry[/] for [yellow]'{project}'[/] in week of [yellow]{weekStart.ToString(DateDisplayFormat)}[/]";

    public static string Separator = new('=', 50);

    public const string ListLogsTitle = "[purple]Available dev-log files:[/]";

    public const string ViewLogSucceeded = "Dev log retrieved.";

    public const string AddEntrySuccess = "Dev-log entry added successfully.";

    public const string EditEntrySuccess = "Dev-log entry updated successfully.";

    public static string ListLogsEmpty(string folder) => $"No dev-log files found in '{folder}'.";

    public static string InvalidLineNumber(int count) => $"[red]Invalid line number. Please enter 1-{count}.[/]";

    public const string ListLogsSucceeded = "Add dev logs retrieved.";

    public static class Errors
    {
        public static readonly Error FolderPathRequired =
            Error.Validation("FolderPath.Required", "The log folder path is required.");

        public static readonly Error ProjectNameRequired =
            Error.Validation("ProjectName.Required", "The project name is required.");

        public static Error FileNotFound(string path) =>
            Error.NotFound("File.NotFound", $"No dev-log file found for the specified week at '{path}'.");
    }
}
