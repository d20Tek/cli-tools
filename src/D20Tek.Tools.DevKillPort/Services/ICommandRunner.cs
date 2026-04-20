namespace D20Tek.Tools.DevKillPort.Services;

internal interface ICommandRunner
{
    Task<string> RunAsync(string command, string arguments, CancellationToken ct = default);
}
