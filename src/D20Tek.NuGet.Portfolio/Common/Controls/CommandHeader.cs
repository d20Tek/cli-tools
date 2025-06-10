namespace D20Tek.NuGet.Portfolio.Common.Controls;

internal class CommandHeader
{
    private readonly IAnsiConsole _console;

    public CommandHeader(IAnsiConsole console) => _console = console;

    public void Render(string title, string color = "grey")
    {
        var rule = new Rule(title)
            .RuleStyle(color)
            .LeftJustified();

        _console.Write(new Padder(rule, new Padding(0, 0, 70, 0)));
    }
}
