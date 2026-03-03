namespace D20Tek.Tools.DevLog.Commands;

internal class EntrySettings : FolderSettings
{
    [CommandOption("-d|--date")]
    [Description("The date for the dev-log entry (defaults to today). Format: MM-dd-yyyy.")]
    public string Date { get; init; } = string.Empty;
}

internal class FolderSettings : CommandSettings
{
    [CommandOption("-f|--folder")]
    [Description("The folder path where dev-log files are stored.")]
    public string Folder { get; init; } = Constants.DefaultLogFolder;
}
