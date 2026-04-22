using D20Tek.Tools.DevKillPort.Contracts;
using D20Tek.Tools.DevKillPort.Services;

namespace D20Tek.Tools.DevKillPort.Commands;

internal sealed class KillPortCommand(
    IPortResolver resolver,
    IProcessTerminator terminator,
    IAnsiConsole console) : AsyncCommand<PortSettings>
{
    private readonly IPortResolver _resolver = resolver;
    private readonly IProcessTerminator _terminator = terminator;
    private readonly IAnsiConsole _console = console;

    protected override async Task<int> ExecuteAsync(CommandContext context, PortSettings settings, CancellationToken ct)
    {
        var options = settings.ToQueryOptions();
        var port = settings.Port;

        var processes = await GetProcesses(port, options, ct);
        if (processes.Count == 0)
        {
            _console.MarkupLine(string.Format(Constants.NoProcessFoundMessage, port));
            return 0;
        }

        RenderOutput(port, options.Json, processes);

        if (options.DryRun)
        {
            RenderDryRun(processes);
            return 0;
        }

        if (!options.Force && !ConfirmKill(processes))
        {
            _console.MarkupLine("[yellow]Kill operation cancelled by user.[/]");
            return 0;
        }

        await KillProcesses(processes, options);

        return options.Watch ? await WatchPortAsync(port, options) : 0;
    }

    private async Task<IReadOnlyList<PortProcessInfo>> GetProcesses(int port, PortQueryOptions options, CancellationToken ct) =>
        await _console.Status()
            .Spinner(Spinner.Known.Default)
            .StartAsync(
                string.Format(Constants.ScanningPortMessage, port),
                _ => _resolver.FindAsync(port, options, ct));

    private void RenderOutput(int port, bool isJson, IReadOnlyList<PortProcessInfo> processes)
    {
        if (isJson)
        {
            OutputHelper.RenderJson(_console, port, processes);
        }
        else
        {
            OutputHelper.RenderTable(_console, port, processes);
        }
    }

    private void RenderDryRun(IReadOnlyList<PortProcessInfo> processes) =>
        processes.ForEach(p => _console.MarkupLine(string.Format(Constants.DryRunMessage, p.ProcessName, p.ProcessId)));

    private bool ConfirmKill(IReadOnlyList<PortProcessInfo> processes)
    {
        var prompt = processes.Count > 1
            ? string.Format(Constants.ConfirmKillAllPrompt, processes.Count)
            : Constants.ConfirmKillPrompt;

        return _console.Confirm(prompt, defaultValue: false);
    }

    private async Task KillProcesses(IReadOnlyList<PortProcessInfo> processes, PortQueryOptions options)
    {
        var processesToKill = options.All ? processes : [processes[0]];

        foreach (var proc in processesToKill)
        {
            var killed = await _terminator.KillAsync(proc.ProcessId, options.Force);
            if (killed)
            {
                _console.MarkupLine(string.Format(Constants.KillSuccessMessage, proc.ProcessName, proc.ProcessId));
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
    }

    private async Task<int> WatchPortAsync(int port, PortQueryOptions options)
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
