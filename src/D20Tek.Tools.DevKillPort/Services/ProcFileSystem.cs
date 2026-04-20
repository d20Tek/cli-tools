using System.Diagnostics.CodeAnalysis;

namespace D20Tek.Tools.DevKillPort.Services;

[ExcludeFromCodeCoverage]
internal sealed class ProcFileSystem : IProcFileSystem
{
    public bool Exists(string path) => File.Exists(path) || Directory.Exists(path);

    public string[] ReadAllLines(string path) => File.ReadAllLines(path);

    public IEnumerable<string> EnumeratePidDirectories()
    {
        try
        {
            return Directory.EnumerateDirectories("/proc")
                            .Where(d => int.TryParse(Path.GetFileName(d), out _));
        }
        catch
        {
            return [];
        }
    }

    public IEnumerable<string> EnumerateFdLinks(string pidPath)
    {
        var fdPath = pidPath + "/fd";
        try
        {
            return Directory.Exists(fdPath) ? Directory.EnumerateFiles(fdPath) : [];
        }
        catch
        {
            return [];
        }
    }

    public string ReadLink(string path)
    {
        try
        {
            var target = File.ResolveLinkTarget(path, returnFinalTarget: false);
            return target?.ToString() ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }
}
