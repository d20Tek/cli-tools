using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace D20Tek.Tools.DevKillPort.Services;

[ExcludeFromCodeCoverage]
internal sealed class ProcessTerminator : IProcessTerminator
{
    public Task<bool> KillAsync(int pid, bool force, CancellationToken ct = default)
    {
        try
        {
            using var process = Process.GetProcessById(pid);
            if (process.HasExited)
                return Task.FromResult(true);

            process.Kill(entireProcessTree: force);
            process.WaitForExit(5000);
            return Task.FromResult(process.HasExited);
        }
        catch (ArgumentException)
        {
            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }
}
