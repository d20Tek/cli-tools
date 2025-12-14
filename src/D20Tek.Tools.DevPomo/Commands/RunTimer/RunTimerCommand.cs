using System.ComponentModel;

namespace D20Tek.Tools.DevPomo.Commands.RunTimer;

internal sealed class RunTimerCommand(IAnsiConsole console, IConfigurationService configurationService) 
    : Command<RunTimerCommand.Settings>
{
    internal class Settings : CommandSettings
    {
        [CommandOption("-c|--cycles <POMODORO-CYCLES>")]
        [Description("Defines how many iterations of pomodoros to run in a session (defaults to 4).")]
        [DefaultValue(Constants.DefaultNumCycles)]
        public int Cycles { get; set; }
    }

    private readonly IAnsiConsole _console = console;
    private readonly IConfigurationService _configurationService = configurationService;

    public override int Execute(CommandContext context, Settings settings, CancellationToken token)
    {
        var timerConfig = _configurationService.Get().GetValue();

        _console.DisplayAppHeader(Constants.AppTitle, timerConfig.ShowAppTitleBar, Justify.Left);
        var state = TimerState.Create(timerConfig);
        using var inputHandler = TimerInputHandler.Start(_console, state);

        state.Iter(s => s.SetPomodoroCycles(settings.Cycles))
             .Map(s => PomodoroEngine.Run(_console, s))
             .Iter(s => ShowExitMessage(_console, s));

        return 0;
    }

    internal static void ShowExitMessage(IAnsiConsole console, TimerState state)
    {
        if (state.Exit)
        {
            console.MarkupLine(Constants.Timer.EarlyStopMsg);
            console.MarkupLinesConditional(
                state.CompletedPomodoros > 0,
                Constants.Timer.EarlyStopPomoCount(state.CompletedPomodoros));
        }
        else
        {
            console.MarkupLine(Constants.Timer.CompletedMsg(state.CompletedPomodoros));
        }
    }
}
