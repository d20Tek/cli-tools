using D20Tek.Spectre.Console.Extensions.Commands;

namespace D20Tek.NuGet.Portfolio.Features;

internal sealed class InteractiveCommand : InteractiveCommandBase
{
    public InteractiveCommand(ICommandApp app, IAnsiConsole console) : base(app, console) { }

    protected override void ShowWelcomeMessage(IAnsiConsole console) =>
        console.ToIdentity().Iter(c => c.Write(new FigletText(Globals.AppTitle).Centered().Color(Color.Green)))
                            .Iter(c => c.MarkupLine(Globals.AppInitializeSuccessMsg))
                            .Iter(c => c.MarkupLine(Globals.AppGetStartedMsg));

    protected override string GetAppPromptPrefix() => Globals.AppPrompt;

    protected override void ShowExitMessage(IAnsiConsole console) => console.WriteLine(Globals.AppExitMessage);
}
