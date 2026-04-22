using D20Tek.Tools.DevKillPort.Contracts;
using D20Tek.Tools.DevKillPort.Services;

namespace D20Tek.Tools.DevKillPort.Commands;

internal sealed class ViewPortCommand(IPortResolver resolver, IAnsiConsole console) : AsyncCommand<PortSettings>
{
    private readonly IPortResolver _resolver = resolver;
    private readonly IAnsiConsole _console = console;

    protected override async Task<int> ExecuteAsync(CommandContext context, PortSettings settings, CancellationToken ct)
    {
        var options = settings.ToQueryOptions();

        var processes = await _console.Status()
            .Spinner(Spinner.Known.Default)
            .StartAsync(
                string.Format(Constants.ScanningPortMessage, settings.Port),
                _ => _resolver.FindAsync(settings.Port, options, ct));

        if (processes.Count == 0)
        {
            _console.MarkupLine(string.Format(Constants.NoProcessFoundMessage, settings.Port));
            return 0;
        }

        RenderOutput(settings.Port, options.Json, processes);
        return 0;
    }

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
}
