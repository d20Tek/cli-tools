using D20Tek.Tools.DevLog.Contracts;

namespace D20Tek.Tools.DevLog.Services;

internal class DevLogService(IFileSystemAdapter fileAdapter) : IDevLogService
{
    private readonly IFileSystemAdapter _fileAdapter = fileAdapter;

    public Result<bool> AddEntry(
        string logFolder, string projectName, List<string> accomplishments, DateOnly? date = null) =>
        Try.Run(() =>
            DevLogEntry.Create(projectName, accomplishments, date)
                .Pipe(entry => (FilePath: GetFilePath(logFolder, entry.WeekStart), Entry: entry))
                .Pipe(t => _fileAdapter.EnsureFolderExists(logFolder).Pipe(_ => t))
                .Pipe(t => _fileAdapter.WriteAllText(t.FilePath, BuildContent(t.FilePath, t.Entry)))
                .Pipe(_ => Result<bool>.Success(true)));

    public Result<string> ViewLog(string logFolder, DateOnly? date = null) =>
        Try.Run(() =>
            DevLogEntry.GetWeekStart(date ?? DateOnly.FromDateTime(DateTime.Today))
                .Pipe(weekStart => GetFilePath(logFolder, weekStart))
                .Pipe(filePath => _fileAdapter.Exists(filePath)
                    ? Result<string>.Success(_fileAdapter.ReadAllText(filePath))
                    : Result<string>.Failure(Constants.Errors.FileNotFound(filePath))));

    public Result<bool> EditEntry(
        string logFolder, string projectName, List<string> accomplishments, DateOnly? date = null) =>
        Try.Run(() =>
            DevLogEntry.Create(projectName, accomplishments, date)
                .Pipe(entry => (FilePath: GetFilePath(logFolder, entry.WeekStart), Entry: entry))
                .Pipe(t => _fileAdapter.Exists(t.FilePath)
                    ? Result<(string FilePath, DevLogEntry Entry)>.Success(t)
                    : Result<(string FilePath, DevLogEntry Entry)>.Failure(
                        Constants.Errors.FileNotFound(t.FilePath)))
                .Bind(t => Try.Run(() =>
                    _fileAdapter.ReadAllText(t.FilePath)
                        .Pipe(content => ReplaceProjectSection(content, t.Entry))
                        .Pipe(updated => _fileAdapter.WriteAllText(t.FilePath, updated))
                        .Pipe(_ => Result<bool>.Success(true)))));

    public Result<IEnumerable<string>> ListLogs(string logFolder) =>
        Try.Run(() =>
            Result<IEnumerable<string>>.Success(
                _fileAdapter.EnumerateFolderFiles(logFolder, Constants.MarkdownSearchPattern)));

    private static string GetFilePath(string logFolder, DateOnly weekStart) =>
        Path.Combine(logFolder, string.Format(Constants.FileNameFormat, weekStart));

    private string BuildContent(string filePath, DevLogEntry entry) =>
        _fileAdapter.Exists(filePath)
            ? _fileAdapter.ReadAllText(filePath) + "\n\n" + GenerateProjectSection(entry)
            : GenerateFileContent(entry);

    private static string GenerateFileContent(DevLogEntry entry) =>
        $"{Constants.WeekHeader(entry.WeekStart)}\n\n{GenerateProjectSection(entry)}";

    private static string GenerateProjectSection(DevLogEntry entry) =>
        $"{Constants.ProjectHeader(entry.ProjectName)}\n" +
        string.Join("\n", entry.Accomplishments.Select(a => $"{Constants.AccomplishmentBullet}{a}"));

    private static string ReplaceProjectSection(string content, DevLogEntry entry)
    {
        var lines = content.Split('\n').ToList();
        var projectHeader = Constants.ProjectHeader(entry.ProjectName);
        var startIndex = lines.FindIndex(l => l.TrimEnd() == projectHeader);

        if (startIndex == -1) return content + "\n\n" + GenerateProjectSection(entry);

        var endIndex = lines.FindIndex(startIndex + 1, l => l.StartsWith("## ") || l.StartsWith("### "));
        if (endIndex == -1) endIndex = lines.Count;

        while (endIndex > startIndex + 1 && string.IsNullOrWhiteSpace(lines[endIndex - 1]))
            endIndex--;

        lines.RemoveRange(startIndex, endIndex - startIndex);
        lines.InsertRange(startIndex, GenerateProjectSection(entry).Split('\n'));
        return string.Join("\n", lines);
    }
}
