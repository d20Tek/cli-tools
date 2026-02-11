using System.Text.Json;

namespace D20Tek.Tools.JsonMinify.Services;

internal class MinifyService(IFileAdapter fileAdapter) : IMinifyService
{
    private static readonly JsonSerializerOptions _options = new() { WriteIndented = false };
    private readonly IFileAdapter _fileAdapter = fileAdapter;

    public Result<bool> MinifyFile(string filePath) =>
        Try.Run(() =>
            _fileAdapter.ReadAllText(filePath)
                .Pipe(json => JsonDocument.Parse(json))
                .Pipe(doc => JsonSerializer.Serialize(doc, _options))
                .Pipe(minified => _fileAdapter.WriteAllText(
                    filePath.Replace(Constants.JsonFileExtension, Constants.MinifiedJsonFileExtension),
                    minified))
                .Pipe(_ => Result<bool>.Success(true)));
}
