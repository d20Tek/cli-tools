using System.Text.Json;

namespace D20Tek.Tools.JsonMinify.Services;

internal class MinifyService(IFileAdapter fileAdapter) : IMinifyService
{
    private static readonly JsonSerializerOptions _options = new() { WriteIndented = false };
    private readonly IFileAdapter _fileAdapter = fileAdapter;

    public Result<int> MinifyFile(string filePath) =>
        Validate(filePath)
            .Bind(f => ProcessFile(f));

    private Result<int> ProcessFile(string filePath) =>
        Try.Run(() =>
        {
            var json = _fileAdapter.ReadAllText(filePath);

            var doc = JsonDocument.Parse(json);
            var minified = JsonSerializer.Serialize(doc, _options);

            _fileAdapter.WriteAllText(
                filePath.Replace(Constants.JsonFileExtension, Constants.MinifiedJsonFileExtension),
                minified);

            return Result<int>.Success(0);
        });

    private Result<string> Validate(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            return Result<string>.Failure(Constants.Errors.FilePathRequired);
        }
        if (!_fileAdapter.Exists(filePath))
        {
            return Result<string>.Failure(Constants.Errors.FilePathNotFound(filePath));
        }
        if (Path.GetExtension(filePath) != Constants.JsonFileExtension)
        {
            return Result<string>.Failure(Constants.Errors.FilePathInvalidExtension(filePath));
        }
        return Result<string>.Success(filePath);
    }
}
