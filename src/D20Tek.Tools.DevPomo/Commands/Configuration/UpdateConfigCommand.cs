namespace D20Tek.Tools.DevPomo.Commands.Configuration;

internal class UpdateConfigCommand(IAnsiConsole console, IConfigurationService service) : Command
{
    private readonly IAnsiConsole _console = console;
    private readonly IConfigurationService _service = service;

    public override int Execute(CommandContext context, CancellationToken token)
    {
        var prevConfig = _service.Get().GetValue();

        _console.DisplayAppHeader("dev-pomo", prevConfig.ShowAppTitleBar, Justify.Left);
        _console.MarkupLines(string.Empty, "Update the configuration for the pomodoro timers.");

        var pomoMinutes = _console.Prompt<int>(new TextPrompt<int>("Enter the pomodoro duration (in minutes)")
            .DefaultValue(prevConfig.PomodoroMinutes)
            .Validate(v => v >= 5 && v <= 120, "Pomodoro duration must be between 5 and 120 minutes."));

        var breakMinutes = _console.Prompt<int>(new TextPrompt<int>("Enter the break duration (in minutes)")
            .DefaultValue(prevConfig.BreakMinutes)
            .Validate(v => v >= 1 && v <= 20, "Pomodoro break must be between 1 and 20 minutes."));

        var showAppTitle = _console.Confirm("Show the terminal application's title?", prevConfig.ShowAppTitleBar);
        var enableSound = _console.Confirm("Play notification sound on timer competion?", prevConfig.EnableSound);
        var autostartCycle = _console.Confirm(
            "Auto-start new cycle when current one completes?",
            prevConfig.AutostartCycles);
        var minimalOutput = _console.Confirm("Show compact/minimal timer output?", prevConfig.MinimalOutput);

        var result = _service.Set(
            TimerConfiguration.Create(
                pomoMinutes,
                breakMinutes,
                showAppTitle,
                enableSound,
                autostartCycle,
                minimalOutput));

        return 0;
    }
}
