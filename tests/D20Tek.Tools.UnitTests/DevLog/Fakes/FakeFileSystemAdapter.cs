using D20Tek.Tools.Common.Services;

namespace D20Tek.Tools.UnitTests.DevLog.Fakes;

[ExcludeFromCodeCoverage]
internal sealed class FakeFileSystemAdapter(string fileContent = "", string[]? folderFiles = null) : IFileSystemAdapter
{
    private string _fileContent = fileContent;

    public string LastWrittenContent { get; private set; } = string.Empty;

    public bool EnsureFolderExists(string path) => true;

    public IEnumerable<string> EnumerateFolderFiles(string path, string searchPattern) => folderFiles ?? [];

    public bool Exists(string path) => !string.IsNullOrEmpty(_fileContent);

    public bool FolderExists(string path) => folderFiles is not null;

    public string ReadAllText(string path) => _fileContent;

    public void WriteAllText(string path, string contents)
    {
        _fileContent = contents;
        LastWrittenContent = contents;
    }
}
