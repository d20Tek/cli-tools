namespace D20Tek.Tools.JsonMinify.Services;

internal class FileSystemAdapter : IFileSystemAdapter
{
    public bool EnsureFolderExists(string path)
    {
        if (Directory.Exists(path)) return true;

        Directory.CreateDirectory(path);
        return true;
    }

    public IEnumerable<string> EnumerateFolderFiles(string path, string searchPattern) =>
        Directory.EnumerateFiles(path, searchPattern)
                 .Where(file =>
                    file.EndsWith(Constants.MinifiedJsonFileExtension, StringComparison.OrdinalIgnoreCase) is false);

    public bool Exists(string path) => File.Exists(path);

    public bool FolderExists(string path) => Directory.Exists(path);

    public string ReadAllText(string path) => File.ReadAllText(path);

    public void WriteAllText(string path, string contents) => File.WriteAllText(path, contents);
}
