using D20Tek.Tools.JsonMinify.Services;

namespace D20Tek.Tools.JsonMinify.Commands;

internal class FolderPathValidator(IFileAdapter fileAdapter)
{
    private static Result<string> Success(string path) => Result<string>.Success(path);
    private static Result<string> Failure(Error error) => Result<string>.Failure(error);

    public Result<string> Validate(string folderPath) => ValidateNotEmpty(folderPath).Bind(ValidateFolderExists);

    private static Result<string> ValidateNotEmpty(string folderPath) =>
        string.IsNullOrEmpty(folderPath) ? Failure(Constants.Errors.FilePathRequired) : Success(folderPath);

    private Result<string> ValidateFolderExists(string folderPath) =>
        fileAdapter.FolderExists(folderPath) ? Success(folderPath) : Failure(Constants.Errors.FolderPathNotFound(folderPath));

}
