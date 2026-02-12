namespace D20Tek.Tools.JsonMinify.Services;

internal interface IMinifyService
{
    Result<bool> MinifyFile(string filePath, string targetFolder = "");
}
