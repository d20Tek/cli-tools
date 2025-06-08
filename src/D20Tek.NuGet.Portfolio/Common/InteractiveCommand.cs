namespace D20Tek.NuGet.Portfolio.Common;

internal class InteractiveCommand : AsyncCommand
{
    private readonly IAnsiConsole _console;

    public InteractiveCommand(IAnsiConsole console) => _console = console;

    public override Task<int> ExecuteAsync(CommandContext context)
    {
        Initialize(context);
        return Task.FromResult(0);
    }

    private void Initialize(CommandContext context) =>
        _console.Write(
            new FigletText(Globals.AppTitle)
                .Centered()
                .Color(Color.Green));
}
