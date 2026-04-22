using D20Tek.Tools.DevKillPort.Contracts;
using System.ComponentModel;

namespace D20Tek.Tools.DevKillPort.Commands;

internal class PortSettings : CommandSettings
{
    [CommandArgument(0, "<PORT>")]
    [Description("The port number to find processes for.")]
    public int Port { get; init; }

    [CommandOption("--force | -f")]
    [Description("Kill without confirmation.")]
    public bool Force { get; init; }

    [CommandOption("--all")]
    [Description("Kill all matching processes.")]
    public bool All { get; init; }

    [CommandOption("--protocol | -p")]
    [Description("Protocol filter: tcp, udp, or both (default: both).")]
    [DefaultValue("both")]
    public string Protocol { get; init; } = "both";

    [CommandOption("--json | -j")]
    [Description("Output results as JSON.")]
    public bool Json { get; init; }

    [CommandOption("--dry-run | -d")]
    [Description("Show what would happen without killing.")]
    public bool DryRun { get; init; }

    [CommandOption("--watch | -w")]
    [Description("Wait until the port is free.")]
    public bool Watch { get; init; }

    [CommandOption("--timeout | -t")]
    [Description("Max wait time in seconds for watch mode (default: 30).")]
    [DefaultValue(30)]
    public int Timeout { get; init; } = 30;

    public PortQueryOptions ToQueryOptions() => new(ParseProtocol(), Force, All, DryRun, Json, Watch, Timeout);

    private ProtocolType ParseProtocol() => Protocol.ToLowerInvariant() switch
    {
        "tcp" => ProtocolType.Tcp,
        "udp" => ProtocolType.Udp,
        _ => ProtocolType.Both
    };
}
