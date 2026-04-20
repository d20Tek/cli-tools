using D20Tek.Spectre.Console.Extensions.Commands;

namespace D20Tek.Tools.DevKillPort.Commands;

internal sealed class InteractiveCommand(ICommandApp app, IAnsiConsole console) :
    InteractiveCommandBase(app, console)
{
    protected override void ShowWelcomeMessage(IAnsiConsole console) =>
        console.ToIdentity().Iter(c => c.Clear())
                            .Iter(c => c.Write(new FigletText(Constants.AppName).Centered().Color(Color.Red)))
                            .Iter(c => c.MarkupLine(Constants.AppInitializeSuccessMsg))
                            .Iter(c => c.MarkupLine(Constants.AppGetStartedMsg));

    protected override string GetAppPromptPrefix() => Constants.AppPrompt;

    protected override void ShowExitMessage(IAnsiConsole console) => console.MarkupLine(Constants.AppExitMessage);
}
