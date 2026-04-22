using D20Tek.Tools.DevKillPort.Contracts;
using System.Diagnostics.CodeAnalysis;

namespace D20Tek.Tools.DevKillPort.Services;

internal sealed class ProcNetParser(IProcFileSystem procFs)
{
    private readonly IProcFileSystem _procFs = procFs;

    internal List<ProcSocketEntry> FindSocketsByPort(int port, ProtocolType protocolType)
    {
        var results = new List<ProcSocketEntry>();

        if (protocolType is ProtocolType.Tcp or ProtocolType.Both)
        {
            results.AddRange(ParseProcNetFile("/proc/net/tcp", port, "TCP"));
            results.AddRange(ParseProcNetFile("/proc/net/tcp6", port, "TCP"));
        }

        if (protocolType is ProtocolType.Udp or ProtocolType.Both)
        {
            results.AddRange(ParseProcNetFile("/proc/net/udp", port, "UDP"));
            results.AddRange(ParseProcNetFile("/proc/net/udp6", port, "UDP"));
        }

        return results;
    }

    internal List<ProcSocketEntry> FindAllSockets(ProtocolType protocolType)
    {
        var results = new List<ProcSocketEntry>();

        if (protocolType is ProtocolType.Tcp or ProtocolType.Both)
        {
            results.AddRange(ParseProcNetFileAll("/proc/net/tcp", "TCP"));
            results.AddRange(ParseProcNetFileAll("/proc/net/tcp6", "TCP"));
        }

        if (protocolType is ProtocolType.Udp or ProtocolType.Both)
        {
            results.AddRange(ParseProcNetFileAll("/proc/net/udp", "UDP"));
            results.AddRange(ParseProcNetFileAll("/proc/net/udp6", "UDP"));
        }

        return results;
    }

    internal List<ProcSocketEntry> ParseProcNetFileAll(string path, string protocol)
    {
        var results = new List<ProcSocketEntry>();
        if (!_procFs.Exists(path)) return results;

        string[] lines = _procFs.ReadAllLines(path);
        foreach (var line in lines.Skip(1))
        {
            var entry = ParseLineAll(line, protocol);
            if (entry is not null)
            {
                results.Add(entry);
            }
        }

        return results;
    }

    internal List<ProcSocketEntry> ParseProcNetFile(string path, int port, string protocol)
    {
        var results = new List<ProcSocketEntry>();
        if (!_procFs.Exists(path)) return results;

        string[] lines = _procFs.ReadAllLines(path);
        foreach (var line in lines.Skip(1))
        {
            var entry = ParseLine(line, port, protocol);
            if (entry is not null)
            {
                results.Add(entry);
            }
        }

        return results;
    }

    internal static ProcSocketEntry? ParseLine(string line, int port, string protocol)
    {
        var parts = line.Split([' '], StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 10) return null;

        var localAddress = parts[1];
        var colonIndex = localAddress.LastIndexOf(':');
        if (colonIndex < 0) return null;

        var portHex = localAddress[(colonIndex + 1)..];
        if (!TryParseHexPort(portHex, out var localPort) || localPort != port) return null;

        var stateHex = parts[3];
        var state = ParseTcpState(stateHex);

        var address = ParseHexAddress(localAddress[..colonIndex]);

        if (!long.TryParse(parts[9], out var inode) || inode == 0) return null;

        return new ProcSocketEntry(port, inode, protocol, address, state);
    }

    internal static bool TryParseHexPort(string hex, out int port)
    {
        port = 0;
        try
        {
            port = Convert.ToInt32(hex, 16);
            return true;
        }
        catch
        {
            return false;
        }
    }

    internal static PortState ParseTcpState(string hexState) =>
        hexState.ToUpperInvariant() switch
        {
            "0A" => PortState.Listen,
            "01" => PortState.Established,
            "06" => PortState.TimeWait,
            "08" => PortState.CloseWait,
            _ => PortState.Other
        };

    private static string ParseHexAddress(string hex)
    {
        if (hex.Length == 8)
        {
            var bytes = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                bytes[i] = Convert.ToByte(hex.Substring((3 - i) * 2, 2), 16);
            }
            return $"{bytes[0]}.{bytes[1]}.{bytes[2]}.{bytes[3]}";
        }

        return hex.Length == 32 ? "[::]" : hex;
    }

    [ExcludeFromCodeCoverage]
    internal static ProcSocketEntry? ParseLineAll(string line, string protocol)
    {
        var parts = line.Split([' '], StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 10) return null;

        var localAddress = parts[1];
        var colonIndex = localAddress.LastIndexOf(':');
        if (colonIndex < 0) return null;

        var portHex = localAddress[(colonIndex + 1)..];
        if (!TryParseHexPort(portHex, out var localPort) || localPort == 0) return null;

        var stateHex = parts[3];
        var state = ParseTcpState(stateHex);

        var address = ParseHexAddress(localAddress[..colonIndex]);

        if (!long.TryParse(parts[9], out var inode) || inode == 0) return null;

        return new ProcSocketEntry(localPort, inode, protocol, address, state);
    }
}

internal record ProcSocketEntry(int Port, long Inode, string Protocol, string Address, PortState State);
