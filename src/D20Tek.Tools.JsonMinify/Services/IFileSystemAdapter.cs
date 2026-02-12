namespace D20Tek.Tools.JsonMinify.Services;

internal interface IFileSystemAdapter
{
    bool EnsureFolderExists(string path);

    IEnumerable<string> EnumerateFolderFiles(string path, string searchPattern);
    
    bool Exists(string path);

    bool FolderExists(string path);

    string ReadAllText(string path);

    void WriteAllText(string path, string contents);
}
