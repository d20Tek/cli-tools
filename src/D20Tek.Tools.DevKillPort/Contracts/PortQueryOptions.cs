namespace D20Tek.Tools.DevKillPort.Contracts;

internal record PortQueryOptions(
    ProtocolType Protocol = ProtocolType.Both,
    bool Force = false,
    bool All = false,
    bool DryRun = false,
    bool Json = false,
    bool Watch = false,
    int Timeout = 30);
