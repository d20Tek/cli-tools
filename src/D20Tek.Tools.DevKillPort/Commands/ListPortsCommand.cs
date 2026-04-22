using D20Tek.Tools.DevKillPort.Services;

namespace D20Tek.Tools.DevKillPort.Commands;

internal sealed class ListPortsCommand(IPortResolver resolver, IAnsiConsole console) : AsyncCommand<ListSettings>
{
    private readonly IPortResolver _resolver = resolver;
    private readonly IAnsiConsole _console = console;

    protected override async Task<int> ExecuteAsync(CommandContext context, ListSettings settings, CancellationToken ct)
    {
        var options = settings.ToQueryOptions();

        var processes = await _console.Status()
            .Spinner(Spinner.Known.Default)
            .StartAsync(Constants.ScanningPortsMessage, _ => _resolver.ListAllAsync(options, ct));

        if (processes.Count == 0)
        {
            _console.MarkupLine(Constants.NoPortsFoundMessage);
            return 0;
        }

        if (options.Json)
        {
            OutputHelper.RenderAllPortsJson(_console, processes);
        }
        else
        {
            OutputHelper.RenderAllPortsTable(_console, processes);
        }

        return 0;
    }
}
