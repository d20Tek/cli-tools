using D20Tek.Tools.DevKillPort.Contracts;

namespace D20Tek.Tools.DevKillPort.Services;

internal sealed class MacPortResolver(ICommandRunner commandRunner) : IPortResolver
{
    private readonly ICommandRunner _commandRunner = commandRunner;

    public async Task<IReadOnlyList<PortProcessInfo>> ListAllAsync(
        PortQueryOptions options,
        CancellationToken ct = default)
    {
        var protocolFilter = options.Protocol switch
        {
            ProtocolType.Tcp => "TCP",
            ProtocolType.Udp => "UDP",
            _ => ""
        };

        var output = await _commandRunner.RunAsync("lsof", "-i -n -P", ct);
        return ParseLsofAllOutput(output, protocolFilter);
    }

    internal static List<PortProcessInfo> ParseLsofAllOutput(string output, string protocolFilter)
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

            var protocol = parts[7].ToUpperInvariant();
            if (!string.IsNullOrEmpty(protocolFilter) &&
                !protocol.Contains(protocolFilter, StringComparison.OrdinalIgnoreCase))
                continue;

            var nameField = parts[8];
            var port = ExtractPortFromNameField(nameField);

            var state = parts.Length > 9 && parts[^1].Contains("LISTEN", StringComparison.OrdinalIgnoreCase)
                ? PortState.Listen
                : parts.Length > 9 && parts[^1].Contains("ESTABLISHED", StringComparison.OrdinalIgnoreCase)
                    ? PortState.Established
                    : PortState.Other;

            results.Add(new PortProcessInfo(port, pid, processName, protocol, "*", state));
        }

        return [.. results.GroupBy(r => (r.ProcessId, r.Protocol, r.Port)).Select(g => g.First())];
    }

    private static int ExtractPortFromNameField(string nameField)
    {
        var lastColon = nameField.LastIndexOf(':');
        if (lastColon >= 0)
        {
            var portPart = nameField[(lastColon + 1)..].Split('-', '>')[0];
            if (int.TryParse(portPart, out var port)) return port;
        }

        return 0;
    }

    public async Task<IReadOnlyList<PortProcessInfo>> FindAsync(
        int port,
        PortQueryOptions options,
        CancellationToken ct = default)
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

            var protocol = parts[7].ToUpperInvariant();
            if (!string.IsNullOrEmpty(protocolFilter) &&
                !protocol.Contains(protocolFilter, StringComparison.OrdinalIgnoreCase))
                continue;

            var nameField = parts[^1];
            var state = nameField.Contains("LISTEN", StringComparison.OrdinalIgnoreCase)
                ? PortState.Listen
                : nameField.Contains("ESTABLISHED", StringComparison.OrdinalIgnoreCase)
                    ? PortState.Established
                    : PortState.Other;

            results.Add(new PortProcessInfo(port, pid, processName, protocol, "*", state));
        }

        return [.. results.GroupBy(r => (r.ProcessId, r.Protocol)).Select(g => g.First())];
    }
}
