using D20Tek.Tools.DevLog.Contracts;

namespace D20Tek.Tools.DevLog.Services;

internal class DevLogService(IFileSystemAdapter fileAdapter) : IDevLogService
{
    private readonly IFileSystemAdapter _fileAdapter = fileAdapter;

    public Result<bool> AddEntry(
        string logFolder,
        string projectName,
        List<string> accomplishments,
        DateOnly? date = null) =>
        Try.Run(() =>
            DevLogEntry.Create(projectName, accomplishments, date)
                .Pipe(entry => (FilePath: GetFilePath(logFolder, entry.WeekStart), Entry: entry))
                .Pipe(t => _fileAdapter.EnsureFolderExists(logFolder).Pipe(_ => t))
                .Pipe(t => _fileAdapter.WriteAllText(
                    t.FilePath, BuildSerializedContent(t.FilePath, t.Entry, MergeEntry)))
                .Pipe(_ => Result<bool>.Success(true)));

    public Result<string> ViewLog(string logFolder, DateOnly? date = null) =>
        Try.Run(() =>
            DevLogEntry.GetWeekStart(date ?? DateOnly.FromDateTime(DateTime.Today))
                .Pipe(weekStart => GetFilePath(logFolder, weekStart))
                .Pipe(filePath => _fileAdapter.Exists(filePath)
                    ? Result<string>.Success(_fileAdapter.ReadAllText(filePath))
                    : Result<string>.Failure(Constants.Errors.FileNotFound(filePath))));

    public Result<bool> EditEntry(
        string logFolder,
        string projectName,
        List<string> accomplishments,
        DateOnly? date = null) =>
        Try.Run(() =>
            DevLogEntry.Create(projectName, accomplishments, date)
                .Pipe(entry => (FilePath: GetFilePath(logFolder, entry.WeekStart), Entry: entry))
                .Pipe(t => _fileAdapter.Exists(t.FilePath)
                    ? Result<(string FilePath, DevLogEntry Entry)>.Success(t)
                    : Result<(string FilePath, DevLogEntry Entry)>.Failure(
                        Constants.Errors.FileNotFound(t.FilePath)))
                .Bind(t => Try.Run(() =>
                    BuildSerializedContent(t.FilePath, t.Entry, UpsertEntry)
                        .Pipe(content => _fileAdapter.WriteAllText(t.FilePath, content))
                        .Pipe(_ => Result<bool>.Success(true)))));

    public Result<IEnumerable<string>> ListLogs(string logFolder) =>
        Try.Run(() =>
            Result<IEnumerable<string>>.Success(
                _fileAdapter.EnumerateFolderFiles(logFolder, Constants.MarkdownSearchPattern)));

    private static string GetFilePath(string logFolder, DateOnly weekStart) =>
        Path.Combine(logFolder, string.Format(Constants.FileNameFormat, weekStart));

    private string BuildSerializedContent(
        string filePath,
        DevLogEntry entry,
        Func<List<DevLogEntry>, DevLogEntry, List<DevLogEntry>> updateFn) =>
        (_fileAdapter.Exists(filePath)
            ? updateFn(MarkdownSerializer.ParseEntries(_fileAdapter.ReadAllText(filePath), entry.WeekStart), entry)
            : [entry])
        .Pipe(entries => MarkdownSerializer.SerializeEntries(entry.WeekStart, entries));

    private static List<DevLogEntry> MergeEntry(List<DevLogEntry> entries, DevLogEntry newEntry) =>
        entries.Any(e => IsSameProject(e, newEntry.ProjectName))
            ? [.. entries.Select(e => IsSameProject(e, newEntry.ProjectName)
                ? e with { Accomplishments = [.. e.Accomplishments, .. newEntry.Accomplishments] }
                : e)]
            : [.. entries, newEntry];

    private static List<DevLogEntry> UpsertEntry(List<DevLogEntry> entries, DevLogEntry newEntry) =>
        entries.Any(e => IsSameProject(e, newEntry.ProjectName))
            ? [.. entries.Select(e => IsSameProject(e, newEntry.ProjectName) ? newEntry : e)]
            : [.. entries, newEntry];

    private static bool IsSameProject(DevLogEntry entry, string projectName) =>
        string.Equals(entry.ProjectName, projectName, StringComparison.OrdinalIgnoreCase);
}
