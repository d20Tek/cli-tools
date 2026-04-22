using D20Tek.Tools.DevKillPort.Contracts;
using System.ComponentModel;

namespace D20Tek.Tools.DevKillPort.Commands;

internal class ListSettings : CommandSettings
{
    [CommandOption("--protocol | -p")]
    [Description("Protocol filter: tcp, udp, or both (default: both).")]
    [DefaultValue("both")]
    public string Protocol { get; init; } = "both";

    [CommandOption("--json | -j")]
    [Description("Output results as JSON.")]
    public bool Json { get; init; }

    public PortQueryOptions ToQueryOptions() => new(ParseProtocol(), false, false, false, Json, false, 30);

    private ProtocolType ParseProtocol() => Protocol.ToLowerInvariant() switch
    {
        "tcp" => ProtocolType.Tcp,
        "udp" => ProtocolType.Udp,
        _ => ProtocolType.Both
    };
}
