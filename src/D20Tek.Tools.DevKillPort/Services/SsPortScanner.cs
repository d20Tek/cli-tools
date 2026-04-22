using D20Tek.Tools.DevKillPort.Contracts;

namespace D20Tek.Tools.DevKillPort.Services;

internal static class SsPortScanner
{
    internal static async Task<List<PortProcessInfo>> TryFindAsync(
        int port,
        PortQueryOptions options,
        ICommandRunner commandRunner,
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

            var output = await commandRunner.RunAsync("ss", $"-lnp {protocolFlag} 'sport = :{port}'", ct);
            return ParseSsOutput(output, port);
        }
        catch
        {
            return [];
        }
    }

    internal static async Task<List<PortProcessInfo>> TryListAllAsync(
        PortQueryOptions options,
        ICommandRunner commandRunner,
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

            var output = await commandRunner.RunAsync("ss", $"-lnp {protocolFlag}", ct);
            return ParseSsAllOutput(output);
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

    internal static PortState ParseState(string state) => state.ToUpperInvariant() switch
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
        [.. results.GroupBy(r => (r.ProcessId, r.Protocol)).Select(g => g.First())];
}
