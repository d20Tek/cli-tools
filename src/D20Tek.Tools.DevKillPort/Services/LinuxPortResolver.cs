using D20Tek.Tools.DevKillPort.Contracts;

namespace D20Tek.Tools.DevKillPort.Services;

internal sealed class LinuxPortResolver(ICommandRunner commandRunner, IProcFileSystem procFs) : IPortResolver
{
    private readonly ICommandRunner _commandRunner = commandRunner;
    private readonly ProcNetParser _procParser = new(procFs);
    private readonly ProcFdScanner _procScanner = new(procFs);

    public async Task<IReadOnlyList<PortProcessInfo>> ListAllAsync(
        PortQueryOptions options,
        CancellationToken ct = default)
    {
        var results = await TryListAllWithSsAsync(options, ct);
        if (results.Count > 0) return results;

        results = await TryListAllWithLsofAsync(options, ct);
        if (results.Count > 0) return results;

        return ListAllWithProc(options);
    }

    private async Task<List<PortProcessInfo>> TryListAllWithSsAsync(
        PortQueryOptions options,
        CancellationToken ct)
    {
        try
        {
            var protocolFlag = options.Protocol switch
            {
                ProtocolType.Tcp => "-t",
                ProtocolType.Udp => "-u",
                _ => "-tu"
            };

            var output = await _commandRunner.RunAsync("ss", $"-lnp {protocolFlag}", ct);
            return ParseSsAllOutput(output);
        }
        catch
        {
            return [];
        }
    }

    private async Task<List<PortProcessInfo>> TryListAllWithLsofAsync(
        PortQueryOptions options,
        CancellationToken ct)
    {
        try
        {
            var protocolFilter = options.Protocol switch
            {
                ProtocolType.Tcp => "TCP",
                ProtocolType.Udp => "UDP",
                _ => ""
            };

            var output = await _commandRunner.RunAsync("lsof", "-i -n -P", ct);
            return ParseLsofOutput(output, 0, protocolFilter)
                .Where(r => r.Port > 0)
                .ToList();
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
                    socket.Port, pidInfo.Pid, pidInfo.ProcessName,
                    socket.Protocol, socket.Address, socket.State));
            }

            return results;
        }
        catch
        {
            return [];
        }
    }

    internal static List<PortProcessInfo> ParseSsAllOutput(string output)
    {
        var results = new List<PortProcessInfo>();
        if (string.IsNullOrWhiteSpace(output)) return results;

        var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var line in lines.Skip(1))
        {
            var parts = line.Split([' ', '\t'], StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 5) continue;

            var state = ParseState(parts[0]);
            var localAddr = parts[3];

            var lastColon = localAddr.LastIndexOf(':');
            if (lastColon < 0) continue;

            var portStr = localAddr[(lastColon + 1)..];
            if (!int.TryParse(portStr, out var port)) continue;

            var address = localAddr[..lastColon];
            var pid = ExtractPidFromSs(parts.Length > 5 ? parts[5] : "");
            var processName = ExtractProcessNameFromSs(parts.Length > 5 ? parts[5] : "");

            if (pid > 0)
            {
                results.Add(new PortProcessInfo(port, pid, processName, "TCP", address, state));
            }
        }

        return DeduplicateByPid(results);
    }

    public async Task<IReadOnlyList<PortProcessInfo>> FindAsync(
        int port,
        PortQueryOptions options,
        CancellationToken ct = default)
    {
        var results = await TryFindWithSsAsync(port, options, ct);
        if (results.Count > 0) return results;

        results = await TryFindWithLsofAsync(port, options, ct);
        if (results.Count > 0) return results;

        return FindWithProc(port, options);
    }

    private async Task<List<PortProcessInfo>> TryFindWithSsAsync(
        int port,
        PortQueryOptions options,
        CancellationToken ct)
    {
        try
        {
            var protocolFlag = options.Protocol switch
            {
                ProtocolType.Tcp => "-t",
                ProtocolType.Udp => "-u",
                _ => "-tu"
            };

            var output = await _commandRunner.RunAsync(
                "ss",
                $"-lnp {protocolFlag} 'sport = :{port}'",
                ct);

            return ParseSsOutput(output, port);
        }
        catch
        {
            return [];
        }
    }

    private async Task<List<PortProcessInfo>> TryFindWithLsofAsync(
        int port,
        PortQueryOptions options,
        CancellationToken ct)
    {
        try
        {
            var protocolFilter = options.Protocol switch
            {
                ProtocolType.Tcp => "TCP",
                ProtocolType.Udp => "UDP",
                _ => ""
            };

            var output = await _commandRunner.RunAsync("lsof", $"-i :{port} -n -P", ct);
            return ParseLsofOutput(output, port, protocolFilter);
        }
        catch
        {
            return [];
        }
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
                    port, pidInfo.Pid, pidInfo.ProcessName,
                    socket.Protocol, socket.Address, socket.State));
            }

            return results;
        }
        catch
        {
            return [];
        }
    }

    internal static List<PortProcessInfo> ParseSsOutput(string output, int port)
    {
        var results = new List<PortProcessInfo>();
        if (string.IsNullOrWhiteSpace(output)) return results;

        var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var line in lines.Skip(1))
        {
            var parts = line.Split([' ', '\t'], StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 5) continue;

            var state = ParseState(parts[0]);
            var localAddr = parts[3];

            var lastColon = localAddr.LastIndexOf(':');
            if (lastColon < 0) continue;

            var portStr = localAddr[(lastColon + 1)..];
            if (!int.TryParse(portStr, out var parsedPort) || parsedPort != port) continue;

            var address = localAddr[..lastColon];
            var pid = ExtractPidFromSs(parts.Length > 5 ? parts[5] : "");
            var processName = ExtractProcessNameFromSs(parts.Length > 5 ? parts[5] : "");

            if (pid > 0)
            {
                results.Add(new PortProcessInfo(port, pid, processName, "TCP", address, state));
            }
        }

        return DeduplicateByPid(results);
    }

    internal static List<PortProcessInfo> ParseLsofOutput(string output, int port, string protocolFilter)
    {
        var results = new List<PortProcessInfo>();
        if (string.IsNullOrWhiteSpace(output)) return results;

        var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var line in lines.Skip(1))
        {
            var parts = line.Split([' ', '\t'], StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 9) continue;

            var processName = parts[0];
            if (!int.TryParse(parts[1], out var pid)) continue;

            var type = parts[4];
            var protocol = type.Contains("IPv") ? parts[7] : type;
            if (!string.IsNullOrEmpty(protocolFilter) &&
                !protocol.Contains(protocolFilter, StringComparison.OrdinalIgnoreCase))
                continue;

            var nameField = parts[^1];
            var state = nameField.Contains("LISTEN", StringComparison.OrdinalIgnoreCase)
                ? PortState.Listen
                : nameField.Contains("ESTABLISHED", StringComparison.OrdinalIgnoreCase)
                    ? PortState.Established
                    : PortState.Other;

            results.Add(new PortProcessInfo(port, pid, processName, protocol.ToUpperInvariant(), "*", state));
        }

        return DeduplicateByPid(results);
    }

    internal static PortState ParseState(string state) =>
        state.ToUpperInvariant() switch
        {
            "LISTEN" => PortState.Listen,
            "ESTAB" or "ESTABLISHED" => PortState.Established,
            "TIME-WAIT" or "TIME_WAIT" or "TIMEWAIT" => PortState.TimeWait,
            "CLOSE-WAIT" or "CLOSE_WAIT" or "CLOSEWAIT" => PortState.CloseWait,
            _ => PortState.Other
        };

    internal static int ExtractPidFromSs(string field)
    {
        var pidStart = field.IndexOf("pid=", StringComparison.Ordinal);
        if (pidStart < 0) return 0;

        pidStart += 4;
        var pidEnd = field.IndexOfAny([',', ')'], pidStart);
        if (pidEnd < 0) pidEnd = field.Length;

        return int.TryParse(field[pidStart..pidEnd], out var pid) ? pid : 0;
    }

    internal static string ExtractProcessNameFromSs(string field)
    {
        var start = field.IndexOf("((\"", StringComparison.Ordinal);
        if (start < 0) return "<unknown>";

        start += 3;
        var end = field.IndexOf('"', start);
        return end > start ? field[start..end] : "<unknown>";
    }

    private static List<PortProcessInfo> DeduplicateByPid(List<PortProcessInfo> results) =>
        results.GroupBy(r => (r.ProcessId, r.Protocol))
               .Select(g => g.First())
               .ToList();
}
