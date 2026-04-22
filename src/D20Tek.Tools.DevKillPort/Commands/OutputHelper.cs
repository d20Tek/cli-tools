using D20Tek.Tools.DevKillPort.Contracts;
using Spectre.Console;
using System.Text.Json;

namespace D20Tek.Tools.DevKillPort.Commands;

internal static class OutputHelper
{
    private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

    public static void RenderTable(IAnsiConsole console, int port, IReadOnlyList<PortProcessInfo> processes)
    {
        console.MarkupLine(string.Format(Constants.PortHeaderMessage, port));
        console.WriteLine();

        var table = new Table()
            .AddColumn("PID")
            .AddColumn("Name")
            .AddColumn("Protocol")
            .AddColumn("State");

        foreach (var p in processes)
        {
            table.AddRow(
                p.ProcessId.ToString(),
                Markup.Escape(p.ProcessName),
                p.Protocol,
                p.State.ToString());
        }

        console.Write(table);
    }

    public static void RenderJson(IAnsiConsole console, int port, IReadOnlyList<PortProcessInfo> processes)
    {
        var output = new
        {
            port,
            processes = processes.Select(p => new
            {
                pid = p.ProcessId,
                name = p.ProcessName,
                protocol = p.Protocol,
                state = p.State.ToString()
            })
        };

        console.WriteLine(JsonSerializer.Serialize(output, _jsonOptions));
    }

    public static void RenderAllPortsTable(IAnsiConsole console, IReadOnlyList<PortProcessInfo> processes)
    {
        console.MarkupLine(Constants.AllPortsHeaderMessage);
        console.WriteLine();

        var table = new Table()
            .AddColumn("Port")
            .AddColumn("PID")
            .AddColumn("Name")
            .AddColumn("Protocol")
            .AddColumn("State");

        foreach (var p in processes.OrderBy(p => p.Port))
        {
            table.AddRow(
                p.Port.ToString(),
                p.ProcessId.ToString(),
                Markup.Escape(p.ProcessName),
                p.Protocol,
                p.State.ToString());
        }

        console.Write(table);
    }

    public static void RenderAllPortsJson(IAnsiConsole console, IReadOnlyList<PortProcessInfo> processes)
    {
        var output = new
        {
            processes = processes.OrderBy(p => p.Port).Select(p => new
            {
                port = p.Port,
                pid = p.ProcessId,
                name = p.ProcessName,
                protocol = p.Protocol,
                state = p.State.ToString()
            })
        };

        console.WriteLine(JsonSerializer.Serialize(output, _jsonOptions));
    }
}
