namespace D20Tek.Tools.DevPomo.Commands.Configuration;

internal sealed class UpdateConfigCommand(IAnsiConsole console, IConfigurationService service) : Command
{
    private readonly IAnsiConsole _console = console;
    private readonly IConfigurationService _service = service;

    public override int Execute(CommandContext context, CancellationToken token)
    {
        var prevConfig = _service.Get().GetValue();

        _console.DisplayAppHeader(Constants.AppTitle, prevConfig.ShowAppTitleBar, Justify.Left);
        _console.MarkupLines(string.Empty, Constants.Configuration.Description);

        var pomoMinutes = _console.Prompt(new TextPrompt<int>(Constants.Configuration.PomodoroMinutesLabel)
            .DefaultValue(prevConfig.PomodoroMinutes)
            .Validate(v => v >= 5 && v <= 120, Constants.Configuration.PomodoroMinutesError));

        var breakMinutes = _console.Prompt(new TextPrompt<int>(Constants.Configuration.BreakMinutesLabel)
            .DefaultValue(prevConfig.BreakMinutes)
            .Validate(v => v >= 1 && v <= 20, Constants.Configuration.BreakMinutesError));

        var showAppTitle = _console.Confirm(Constants.Configuration.ShowAppTitleLabel, prevConfig.ShowAppTitleBar);
        var enableSound = _console.Confirm(Constants.Configuration.PlaySoundLabel, prevConfig.EnableSound);
        var autostartCycle = _console.Confirm(
            Constants.Configuration.AutoStartCyclesLabel,
            prevConfig.AutostartCycles);
        var minimalOutput = _console.Confirm(Constants.Configuration.BreakMinutesError, prevConfig.MinimalOutput);

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
