namespace D20Tek.NuGet.Portfolio.Common.Controls;

internal class CommandHeader
{
    private const string _defaultColor = "grey";
    private const int _paddingRight = 70;
    private readonly IAnsiConsole _console;

    public CommandHeader(IAnsiConsole console) => _console = console;

    public void Render(string title, string color = _defaultColor, int rightPad = _paddingRight) =>
        _console.Write(new Padder(
                        new Rule(title).RuleStyle(color).LeftJustified(),
                        new Padding(0, 0, rightPad, 0)));
}
