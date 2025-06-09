using D20Tek.NuGet.Portfolio.Persistence;
using D20Tek.Spectre.Console.Extensions.Commands;

namespace D20Tek.NuGet.Portfolio.Features;

internal sealed class InteractiveCommand : InteractiveCommandBase
{
    private readonly AppDbContext _dbContext;

    public InteractiveCommand(ICommandApp app, IAnsiConsole console, AppDbContext dbContext) : base(app, console)
    {
        _dbContext = dbContext;
    }

    protected override void ShowWelcomeMessage(IAnsiConsole console) =>
        console.ToIdentity().Iter(_dbContext.ApplyMigrations)
                            .Iter(c => c.Clear())
                            .Iter(c => c.Write(new FigletText(Globals.AppTitle).Centered().Color(Color.Green)))
                            .Iter(c => c.MarkupLine(Globals.AppInitializeSuccessMsg))
                            .Iter(c => c.MarkupLine(Globals.AppGetStartedMsg));

    protected override string GetAppPromptPrefix() => Globals.AppPrompt;

    protected override void ShowExitMessage(IAnsiConsole console) => console.WriteLine(Globals.AppExitMessage);
}
