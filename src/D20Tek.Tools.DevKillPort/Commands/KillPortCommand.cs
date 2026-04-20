using D20Tek.Tools.DevKillPort.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace D20Tek.Tools.DevKillPort.Commands;

internal sealed class KillPortCommand(
    IPortResolver resolver,
    IProcessTerminator terminator,
    IAnsiConsole console) : AsyncCommand<PortSettings>
{
    private readonly IPortResolver _resolver = resolver;
    private readonly IProcessTerminator _terminator = terminator;
    private readonly IAnsiConsole _console = console;

    public override async Task<int> ExecuteAsync(CommandContext context, PortSettings settings, CancellationToken _)
    {
        var options = settings.ToQueryOptions();
        var processes = await _resolver.FindAsync(settings.Port, options);

        if (processes.Count == 0)
        {
            _console.MarkupLine(string.Format(Constants.NoProcessFoundMessage, settings.Port));
            return 0;
        }

        if (options.Json)
        {
            OutputHelper.RenderJson(_console, settings.Port, processes);
        }
        else
        {
            OutputHelper.RenderTable(_console, settings.Port, processes);
        }

        if (options.DryRun)
        {
            foreach (var proc in processes)
            {
                _console.MarkupLine(string.Format(Constants.DryRunMessage, proc.ProcessName, proc.ProcessId));
            }
            return 0;
        }

        if (!options.Force)
        {
            var prompt = processes.Count > 1
                ? string.Format(Constants.ConfirmKillAllPrompt, processes.Count)
                : Constants.ConfirmKillPrompt;

            if (!_console.Confirm(prompt, defaultValue: false))
                return 0;
        }

        var processesToKill = options.All ? processes : [processes[0]];

        foreach (var proc in processesToKill)
        {
            var killed = await _terminator.KillAsync(proc.ProcessId, options.Force);
            if (killed)
            {
                _console.MarkupLine(
                    string.Format(Constants.KillSuccessMessage, proc.ProcessName, proc.ProcessId));
            }
            else
            {
                _console.MarkupLine(
                    string.Format(
                        Constants.KillFailedMessage,
                        proc.ProcessName,
                        proc.ProcessId,
                        Constants.PermissionDeniedMessage));
            }
        }

        if (options.Watch)
        {
            return await WatchPortAsync(settings.Port, options);
        }

        return 0;
    }

    private async Task<int> WatchPortAsync(int port, Contracts.PortQueryOptions options)
    {
        _console.MarkupLine(string.Format(Constants.WatchingPortMessage, port));
        var deadline = DateTime.UtcNow.AddSeconds(options.Timeout);

        while (DateTime.UtcNow < deadline)
        {
            await Task.Delay(1000);
            var remaining = await _resolver.FindAsync(port, options);
            if (remaining.Count == 0)
            {
                _console.MarkupLine(string.Format(Constants.WatchPortFreeMessage, port));
                return 0;
            }
        }

        _console.MarkupLine(string.Format(Constants.WatchTimeoutMessage, port, options.Timeout));
        return -1;
    }
}
