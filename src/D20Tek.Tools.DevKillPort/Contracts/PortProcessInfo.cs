namespace D20Tek.Tools.DevKillPort.Contracts;

internal record PortProcessInfo(
    int Port,
    int ProcessId,
    string ProcessName,
    string Protocol,
    string Address,
    PortState State);
