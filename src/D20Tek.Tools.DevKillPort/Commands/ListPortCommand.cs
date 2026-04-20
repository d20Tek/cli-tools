using D20Tek.Tools.DevKillPort.Services;

namespace D20Tek.Tools.DevKillPort.Commands;

internal sealed class ListPortCommand(IPortResolver resolver, IAnsiConsole console) : AsyncCommand<PortSettings>
{
    private readonly IPortResolver _resolver = resolver;
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

        return 0;
    }
}
