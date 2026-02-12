using System.Text.Json;

namespace D20Tek.Tools.JsonMinify.Services;

internal class MinifyService(IFileSystemAdapter fileAdapter) : IMinifyService
{
    private static readonly JsonSerializerOptions _options = new() { WriteIndented = false };
    private readonly IFileSystemAdapter _fileAdapter = fileAdapter;

    public Result<bool> MinifyFile(string filePath, string targetFolder = "") =>
        Try.Run(() =>
            _fileAdapter.ReadAllText(filePath)
                .Pipe(json => JsonDocument.Parse(json))
                .Pipe(doc => JsonSerializer.Serialize(doc, _options))
                .Pipe(minified => _fileAdapter.WriteAllText(GetOutputFilePath(filePath, targetFolder), minified))
                .Pipe(_ => Result<bool>.Success(true)));

    private string GetOutputFilePath(string filePath, string targetFolder) =>
        CalculateOutputFolder(targetFolder)
            .Pipe(folder =>
            {
                _fileAdapter.EnsureFolderExists(folder);
                return folder;
            })
            .Pipe(folder => Path.Combine(
                folder,
                Path.GetFileName(filePath)
                    .Replace(Constants.JsonFileExtension, Constants.MinifiedJsonFileExtension)));

    private static string CalculateOutputFolder(string targetFolder) => 
        string.IsNullOrEmpty(targetFolder) ? Directory.GetCurrentDirectory() : targetFolder;
}
