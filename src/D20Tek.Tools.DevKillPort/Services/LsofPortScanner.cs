using D20Tek.Tools.DevKillPort.Contracts;

namespace D20Tek.Tools.DevKillPort.Services;

internal static class LsofPortScanner
{
    internal static async Task<List<PortProcessInfo>> TryFindAsync(
        int port,
        PortQueryOptions options,
        ICommandRunner commandRunner,
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

            var output = await commandRunner.RunAsync("lsof", $"-i :{port} -n -P", ct);
            return ParseLsofOutput(output, port, protocolFilter);
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
            var protocolFilter = options.Protocol switch
            {
                ProtocolType.Tcp => "TCP",
                ProtocolType.Udp => "UDP",
                _ => ""
            };

            var output = await commandRunner.RunAsync("lsof", "-i -n -P", ct);
            return [.. ParseLsofOutput(output, 0, protocolFilter).Where(r => r.Port > 0)];
        }
        catch
        {
            return [];
        }
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
            if (!string.IsNullOrEmpty(protocolFilter) && !protocol.Contains(protocolFilter, StringComparison.OrdinalIgnoreCase))
                continue;

            var nameField = parts[^1];
            var state = nameField.Contains("LISTEN", StringComparison.OrdinalIgnoreCase)
                ? PortState.Listen
                : nameField.Contains("ESTABLISHED", StringComparison.OrdinalIgnoreCase) ? PortState.Established : PortState.Other;

            results.Add(new PortProcessInfo(port, pid, processName, protocol.ToUpperInvariant(), "*", state));
        }

        return DeduplicateByPid(results);
    }

    private static List<PortProcessInfo> DeduplicateByPid(List<PortProcessInfo> results) =>
        [.. results.GroupBy(r => (r.ProcessId, r.Protocol, r.Port)).Select(g => g.First())];
}
