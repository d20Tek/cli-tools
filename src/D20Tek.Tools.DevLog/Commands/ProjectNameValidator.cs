namespace D20Tek.Tools.DevLog.Commands;

internal static class ProjectNameValidator
{
    public static Result<string> Validate(string projectName) =>
        string.IsNullOrWhiteSpace(projectName)
            ? Result<string>.Failure(Constants.Errors.ProjectNameRequired)
            : Result<string>.Success(projectName);
}
