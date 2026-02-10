namespace D20Tek.Tools.JsonMinify.Services;

internal interface IMinifyService
{
    Result<int> MinifyFile(string filePath);
}
