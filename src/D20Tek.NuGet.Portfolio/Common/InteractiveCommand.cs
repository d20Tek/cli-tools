namespace D20Tek.NuGet.Portfolio.Common;

internal class InteractiveCommand : AsyncCommand
{
    private readonly Identity<IAnsiConsole> _console;
    private readonly ICommandApp _commandApp;

    public InteractiveCommand(ICommandApp app, IAnsiConsole console) =>
        (_commandApp, _console) = (app, console.ToIdentity());

    public override async Task<int> ExecuteAsync(CommandContext context)
    {
        Initialize(_console);

        while (true)
        {
            var commandText = _console.Iter(c => c.WriteLine())
                                      .Map(c => c.Prompt(new TextPrompt<string>(Globals.AppPrompt)));

            if (IsExitCommand(commandText))
                return Globals.S_OK;

            var args = CliParser.SplitCommandLine(commandText);
            var result = await _commandApp.RunAsync(args);

            if (result != Globals.S_OK)
            {
                return result;
            }
        }
    }

    private static void Initialize(Identity<IAnsiConsole> console) =>
        console.Iter(c => c.Write(new FigletText(Globals.AppTitle).Centered().Color(Color.Green)))
               .Iter(c => c.MarkupLine(Globals.AppInitializeSuccessMsg))
               .Iter(c => c.MarkupLine(Globals.AppGetStartedMsg));

    private static bool IsExitCommand(string commandText) =>
        commandText.Equals("exit", StringComparison.OrdinalIgnoreCase) ||
        commandText.Equals("x", StringComparison.OrdinalIgnoreCase);

}
