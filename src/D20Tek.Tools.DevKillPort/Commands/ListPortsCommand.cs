using D20Tek.Tools.DevKillPort.Contracts;
using D20Tek.Tools.DevKillPort.Services;

namespace D20Tek.Tools.DevKillPort.Commands;

internal sealed class ListPortsCommand(IPortResolver resolver, IAnsiConsole console) : AsyncCommand<ListSettings>
{
    private readonly IPortResolver _resolver = resolver;
    private readonly IAnsiConsole _console = console;

    protected override async Task<int> ExecuteAsync(CommandContext context, ListSettings settings, CancellationToken ct)
    {
        var options = settings.ToQueryOptions();

        var processes = await GetProcesses(options, ct);
        if (processes.Count == 0)
        {
            _console.MarkupLine(Constants.NoPortsFoundMessage);
            return 0;
        }

        RenderOutput(options.Json, processes);
        return 0;
    }

    private async Task<IReadOnlyList<PortProcessInfo>> GetProcesses(PortQueryOptions options, CancellationToken ct) =>
        await _console.Status()
            .Spinner(Spinner.Known.Default)
            .StartAsync(Constants.ScanningPortsMessage, _ => _resolver.ListAllAsync(options, ct));

    private void RenderOutput(bool isJson, IReadOnlyList<PortProcessInfo> processes)
    {
        if (isJson)
        {
            OutputHelper.RenderAllPortsJson(_console, processes);
        }
        else
        {
            OutputHelper.RenderAllPortsTable(_console, processes);
        }
    }
}
