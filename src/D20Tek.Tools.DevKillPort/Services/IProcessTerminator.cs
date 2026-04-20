namespace D20Tek.Tools.DevKillPort.Services;

internal interface IProcessTerminator
{
    Task<bool> KillAsync(int pid, bool force, CancellationToken ct = default);
}
