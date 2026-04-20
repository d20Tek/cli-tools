using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace D20Tek.Tools.DevKillPort.Services;

[ExcludeFromCodeCoverage]
internal sealed class CommandRunner : ICommandRunner
{
    public async Task<string> RunAsync(string command, string arguments, CancellationToken ct = default)
    {
        using var process = new Process();
        process.StartInfo = new ProcessStartInfo
        {
            FileName = command,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        process.Start();
        var output = await process.StandardOutput.ReadToEndAsync(ct);
        await process.WaitForExitAsync(ct);

        return output;
    }
}
