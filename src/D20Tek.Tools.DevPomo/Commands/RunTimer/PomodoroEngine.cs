using System.Diagnostics.CodeAnalysis;

namespace D20Tek.Tools.DevPomo.Commands.RunTimer;

[ExcludeFromCodeCoverage]
internal static class PomodoroEngine
{
    public static TimerState Run(IAnsiConsole console, TimerState state)
    {
        for (var i = 0; i < state.PomodoroCycles; i++)
        {
            state.Map(s => s.RunIfNotExited(() => RunPomodoroPhase(console, s)))
                 .Map(s => s.RunIfNotExited(() => RunBreakPhase(console, s)))
                 .Map(s => s.RunIfNotExited(() => ConfirmRestart(console, s)));

            if (state.Exit) return state;
        }

        return state;
    }

    private static TimerState RunIfNotExited(this TimerState state, Func<TimerState> op) => state.Exit ? state : op();

    private static TimerState RunPomodoroPhase(IAnsiConsole console, TimerState state) =>
        state.Iter(_ => console.MarkupLines(GetPomodoroMessages(state)))
             .Map(s => RunTimerPhase(
                             console,
                             s,
                             s.PomodoroMinutes * Constants.Timer.MinuteMultiplier,
                             Constants.Timer.PomodoroDetails)
                           .WithBeep(console, Constants.Timer.EndPomoCycleMsg)
                           .IncrementPomodoro());

    private static string[] GetPomodoroMessages(TimerState state)
    {
        string[] extras =
            state.Configuration.MinimalOutput ? [] : Constants.Timer.PomoStartedFullMsg(state.PomodoroMinutes);
        return [Constants.Timer.PomoStartedMsg, .. extras];
    }

    private static TimerState RunBreakPhase(IAnsiConsole console, TimerState state) =>
        state.Iter(_ => console.MarkupLine(Constants.Timer.StartBreakMsg))
             .Map(s => RunTimerPhase(
                             console,
                             s,
                             s.BreakMinutes * Constants.Timer.MinuteMultiplier,
                             Constants.Timer.BreakDetails)
                           .WithBeep(console, Constants.Timer.EndBreakMsg));

    private static TimerState RunTimerPhase(
        IAnsiConsole console,
        TimerState state,
        int totalSeconds,
        PanelDetails details)
    {
        var panel = new TimerPanel(details, state.Configuration.MinimalOutput);
        var remainingSeconds = totalSeconds;
        var runningState = state.RestartTimer();

        console.Live(panel.Render(remainingSeconds, totalSeconds, runningState.Paused))
               .AutoClear(false)
               .Overflow(VerticalOverflow.Ellipsis)
               .Cropping(VerticalOverflowCropping.Top)
               .Start(ctx =>
               {
                    while (!runningState.Exit && remainingSeconds > 0)
                    {
                        if (!runningState.Paused)
                        {
                            remainingSeconds = Math.Max(totalSeconds - runningState.GetElapsedSeconds(), 0);
                        }

                        ctx.UpdateTarget(panel.Render(remainingSeconds, totalSeconds, runningState.Paused));

                        Thread.Sleep(100);
                    }
               });

        return runningState;
    }

    private static TimerState ConfirmRestart(IAnsiConsole console, TimerState state) =>
        state switch
        {
            { } when state.ArePomodorosComplete() => state,
            { } when state.Configuration.AutostartCycles => state,
            _ when console.Confirm(Constants.Timer.ContinueCyclesLabel) => state!,
            _ => state!.RequestExit()
        };
}
