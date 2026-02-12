using D20Tek.Tools.JsonMinify.Services;

namespace D20Tek.Tools.UnitTests.JsonMinify.Fakes;

internal sealed class FakeFileSystemAdapter(string fileBody, string[]? folderFiles = null) : IFileSystemAdapter
{
    public bool EnsureFolderExists(string path) => true;

    [ExcludeFromCodeCoverage]
    public IEnumerable<string> EnumerateFolderFiles(string path, string searchPattern) => folderFiles ?? [];

    public bool Exists(string path) => string.IsNullOrEmpty(fileBody) is false;

    public bool FolderExists(string path) => folderFiles is not null;

    public string ReadAllText(string path) => fileBody;

    public void WriteAllText(string path, string contents) { }
}
