namespace D20Tek.Tools.DevKillPort.Services;

internal interface IProcFileSystem
{
    bool Exists(string path);

    string[] ReadAllLines(string path);

    IEnumerable<string> EnumeratePidDirectories();

    IEnumerable<string> EnumerateFdLinks(string pidPath);

    string ReadLink(string path);
}
