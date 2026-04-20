using D20Tek.Tools.DevKillPort.Contracts;
using System.Diagnostics.CodeAnalysis;

namespace D20Tek.Tools.DevKillPort.Services;

internal sealed class WindowsPortResolver(ICommandRunner commandRunner) : IPortResolver
{
    private readonly ICommandRunner _commandRunner = commandRunner;

    public async Task<IReadOnlyList<PortProcessInfo>> FindAsync(
        int port,
        PortQueryOptions options,
        CancellationToken ct = default)
    {
        var results = new List<PortProcessInfo>();

        if (options.Protocol is ProtocolType.Tcp or ProtocolType.Both)
        {
            var tcpResults = await FindTcpAsync(port, ct);
            results.AddRange(tcpResults);
        }

        if (options.Protocol is ProtocolType.Udp or ProtocolType.Both)
        {
            var udpResults = await FindUdpAsync(port, ct);
            results.AddRange(udpResults);
        }

        return DeduplicateByPid(results);
    }

    private async Task<List<PortProcessInfo>> FindTcpAsync(int port, CancellationToken ct)
    {
        var output = await _commandRunner.RunAsync(
            "powershell",
            $"-NoProfile -Command \"Get-NetTCPConnection -LocalPort {port} -ErrorAction SilentlyContinue " +
            $"| Select-Object LocalAddress,LocalPort,OwningProcess,State " +
            $"| ConvertTo-Csv -NoTypeInformation\"",
            ct);

        return ParseCsvOutput(output, port, "TCP");
    }

    private async Task<List<PortProcessInfo>> FindUdpAsync(int port, CancellationToken ct)
    {
        var output = await _commandRunner.RunAsync(
            "powershell",
            $"-NoProfile -Command \"Get-NetUDPEndpoint -LocalPort {port} -ErrorAction SilentlyContinue " +
            $"| Select-Object LocalAddress,LocalPort,OwningProcess " +
            $"| ConvertTo-Csv -NoTypeInformation\"",
            ct);

        return ParseCsvOutput(output, port, "UDP");
    }

    internal static List<PortProcessInfo> ParseCsvOutput(string output, int port, string protocol)
    {
        var results = new List<PortProcessInfo>();
        if (string.IsNullOrWhiteSpace(output)) return results;

        var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (lines.Length < 2) return results;

        for (int i = 1; i < lines.Length; i++)
        {
            var fields = lines[i].Split(',');
            if (fields.Length < 3) continue;

            var address = fields[0].Trim('"');
            if (!int.TryParse(fields[2].Trim('"'), out var pid)) continue;

            var state = fields.Length > 3 ? ParseState(fields[3].Trim('"')) : PortState.Listen;
            var processName = GetProcessName(pid);

            results.Add(new PortProcessInfo(port, pid, processName, protocol, address, state));
        }

        return DeduplicateByPid(results);
    }

    internal static PortState ParseState(string state) =>
        state.ToUpperInvariant() switch
        {
            "LISTEN" or "2" => PortState.Listen,
            "ESTABLISHED" or "5" => PortState.Established,
            "TIMEWAIT" or "11" => PortState.TimeWait,
            "CLOSEWAIT" or "8" => PortState.CloseWait,
            _ => PortState.Other
        };

    [ExcludeFromCodeCoverage]
    private static string GetProcessName(int pid)
    {
        try
        {
            using var process = System.Diagnostics.Process.GetProcessById(pid);
            return process.ProcessName;
        }
        catch
        {
            return "<unknown>";
        }
    }

    private static List<PortProcessInfo> DeduplicateByPid(List<PortProcessInfo> results) =>
        results.GroupBy(r => (r.ProcessId, r.Protocol))
               .Select(g => g.First())
               .ToList();
}
