namespace D20Tek.Tools.DevKillPort.Services;

internal sealed class ProcFdScanner(IProcFileSystem procFs)
{
    private readonly IProcFileSystem _procFs = procFs;
    private Dictionary<long, PidInfo>? _inodeToPidCache;

    internal PidInfo? FindPidByInode(long inode)
    {
        var cache = GetOrBuildCache();
        return cache.TryGetValue(inode, out var pidInfo) ? pidInfo : null;
    }

    internal Dictionary<long, PidInfo> GetOrBuildCache()
    {
        if (_inodeToPidCache is not null) return _inodeToPidCache;

        _inodeToPidCache = [];

        foreach (var pidDir in _procFs.EnumeratePidDirectories())
        {
            var pidName = Path.GetFileName(pidDir);
            if (!int.TryParse(pidName, out var pid)) continue;

            var processName = ReadProcessName(pidDir);

            foreach (var fdLink in _procFs.EnumerateFdLinks(pidDir))
            {
                var target = _procFs.ReadLink(fdLink);
                if (string.IsNullOrEmpty(target) is false)
                {
                    if (TryExtractSocketInode(target, out var socketInode))
                    {
                        _inodeToPidCache.TryAdd(socketInode, new PidInfo(pid, processName));
                    }
                }
            }
        }

        return _inodeToPidCache;
    }

    internal void InvalidateCache() => _inodeToPidCache = null;

    private string ReadProcessName(string pidDir)
    {
        var commPath = pidDir + "/comm";
        try
        {
            if (_procFs.Exists(commPath))
            {
                var lines = _procFs.ReadAllLines(commPath);
                if (!string.IsNullOrWhiteSpace(lines.FirstOrDefault()))
                {
                    return lines[0].Trim();
                }
            }
        }
        catch
        {
            // permission denied or process exited
        }

        return "<unknown>";
    }

    internal static bool TryExtractSocketInode(string linkTarget, out long inode)
    {
        inode = 0;

        if (!linkTarget.StartsWith("socket:[", StringComparison.Ordinal)) return false;

        var start = 8;
        var end = linkTarget.IndexOf(']', start);

        return long.TryParse(linkTarget[start..end], out inode) && inode > 0;
    }
}

internal record PidInfo(int Pid, string ProcessName);
