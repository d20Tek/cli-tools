using D20Tek.Tools.DevKillPort.Contracts;

namespace D20Tek.Tools.DevKillPort.Services;

internal sealed class ProcPortScanner
{
    private readonly ProcNetParser _procParser;
    private readonly ProcFdScanner _procScanner;

    internal ProcPortScanner(IProcFileSystem procFs)
    {
        _procParser = new ProcNetParser(procFs);
        _procScanner = new ProcFdScanner(procFs);
    }

    internal List<PortProcessInfo> FindWithProc(int port, PortQueryOptions options)
    {
        try
        {
            var sockets = _procParser.FindSocketsByPort(port, options.Protocol);
            if (sockets.Count == 0) return [];

            var results = new List<PortProcessInfo>();
            var seenPids = new HashSet<(int, string)>();

            foreach (var socket in sockets)
            {
                var pidInfo = _procScanner.FindPidByInode(socket.Inode);
                if (pidInfo is null) continue;

                if (!seenPids.Add((pidInfo.Pid, socket.Protocol))) continue;

                results.Add(new PortProcessInfo(
                    port, pidInfo.Pid, pidInfo.ProcessName, socket.Protocol, socket.Address, socket.State));
            }

            return results;
        }
        catch
        {
            return [];
        }
    }

    internal List<PortProcessInfo> ListAllWithProc(PortQueryOptions options)
    {
        try
        {
            var sockets = _procParser.FindAllSockets(options.Protocol);
            if (sockets.Count == 0) return [];

            var results = new List<PortProcessInfo>();
            var seenPids = new HashSet<(int, string, int)>();

            foreach (var socket in sockets)
            {
                var pidInfo = _procScanner.FindPidByInode(socket.Inode);
                if (pidInfo is null) continue;

                if (!seenPids.Add((pidInfo.Pid, socket.Protocol, socket.Port))) continue;

                results.Add(new PortProcessInfo(
                    socket.Port, pidInfo.Pid, pidInfo.ProcessName, socket.Protocol, socket.Address, socket.State));
            }

            return results;
        }
        catch
        {
            return [];
        }
    }
}
