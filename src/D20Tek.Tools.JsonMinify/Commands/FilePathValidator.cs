using D20Tek.Tools.JsonMinify.Services;

namespace D20Tek.Tools.JsonMinify.Commands;

internal class FilePathValidator(IFileSystemAdapter fileAdapter)
{
    private static Result<string> Success(string filePath) => Result<string>.Success(filePath);
    private static Result<string> Failure(Error error) => Result<string>.Failure(error);

    public Result<string> Validate(string filePath) =>
        ValidateNotEmpty(filePath)
            .Bind(ValidateFileExists)
            .Bind(ValidateJsonExtension);

    private static Result<string> ValidateNotEmpty(string filePath) =>
        string.IsNullOrEmpty(filePath) ? Failure(Constants.Errors.FilePathRequired) : Success(filePath);

    private Result<string> ValidateFileExists(string filePath) =>
        fileAdapter.Exists(filePath) ? Success(filePath) : Failure(Constants.Errors.FilePathNotFound(filePath));

    private static Result<string> ValidateJsonExtension(string filePath) =>
        Path.GetExtension(filePath) == Constants.JsonFileExtension
            ? Success(filePath)
            : Failure(Constants.Errors.FilePathInvalidExtension(filePath));
}
