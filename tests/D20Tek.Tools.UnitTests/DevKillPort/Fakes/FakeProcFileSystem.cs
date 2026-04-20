using D20Tek.Tools.DevKillPort.Services;

namespace D20Tek.Tools.UnitTests.DevKillPort.Fakes;

[ExcludeFromCodeCoverage]
internal sealed class FakeProcFileSystem : IProcFileSystem
{
    private readonly Dictionary<string, string[]> _files = new();
    private readonly Dictionary<string, string> _links = new();
    private readonly HashSet<string> _existingPaths = new();
    private readonly HashSet<string> _throwingExistsPaths = new();
    private readonly HashSet<string> _throwingReadAllLinesPaths = new();
    private List<string> _pidDirectories = [];
    private readonly Dictionary<string, List<string>> _fdLinks = new();

    public FakeProcFileSystem WithFile(string path, string[] lines)
    {
        _files[path] = lines;
        _existingPaths.Add(path);
        return this;
    }

    public FakeProcFileSystem WithPidDirectory(string pidDir)
    {
        _pidDirectories.Add(pidDir);
        _existingPaths.Add(pidDir);
        return this;
    }

    public FakeProcFileSystem WithFdLink(string pidDir, string fdPath, string linkTarget)
    {
        if (!_fdLinks.ContainsKey(pidDir))
            _fdLinks[pidDir] = [];
        _fdLinks[pidDir].Add(fdPath);
        _links[fdPath] = linkTarget;
        _existingPaths.Add(fdPath);
        return this;
    }

    public FakeProcFileSystem WithThrowingExists(string path)
    {
        _throwingExistsPaths.Add(path);
        return this;
    }

    public FakeProcFileSystem WithThrowingReadAllLines(string path)
    {
        _existingPaths.Add(path);
        _throwingReadAllLinesPaths.Add(path);
        return this;
    }

    public bool Exists(string path)
    {
        if (_throwingExistsPaths.Contains(path)) throw new InvalidOperationException("Exists failed.");
        return _existingPaths.Contains(path);
    }

    public string[] ReadAllLines(string path)
    {
        if (_throwingReadAllLinesPaths.Contains(path)) throw new InvalidOperationException("ReadAllLines failed.");
        return _files.TryGetValue(path, out var lines)
            ? lines
            : throw new FileNotFoundException($"File not found: {path}");
    }

    public IEnumerable<string> EnumeratePidDirectories() => _pidDirectories;

    public IEnumerable<string> EnumerateFdLinks(string pidPath) =>
        _fdLinks.TryGetValue(pidPath, out var links) ? links : [];

    public string ReadLink(string path) =>
        _links.TryGetValue(path, out var target) ? target : string.Empty;
}
