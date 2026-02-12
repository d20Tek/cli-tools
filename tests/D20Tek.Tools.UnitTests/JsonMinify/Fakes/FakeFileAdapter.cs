using D20Tek.Tools.JsonMinify.Services;

namespace D20Tek.Tools.UnitTests.JsonMinify.Fakes;

internal sealed class FakeFileAdapter(string fileBody, string[]? folderFiles = null) : IFileSystemAdapter
{
    public IEnumerable<string> EnumerateFolderFiles(string path, string searchPattern) => folderFiles ?? [];

    public bool Exists(string path) => string.IsNullOrEmpty(fileBody) is false;

    public bool FolderExists(string path) => throw new NotImplementedException();

    public string ReadAllText(string path) => fileBody;

    public void WriteAllText(string path, string contents) { }
}
