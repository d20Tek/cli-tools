using D20Tek.Functional;
using D20Tek.Tools.DevPomo.Common;
using Spectre.Console;

namespace D20Tek.Tools.DevPomo.Commands.RunTimer;

internal static class PomodoroEngine
{
    private const int _minuteMultiplier = 60;
    private static readonly PanelDetails _pomodoroDetails = new("🍅 Pomodoro", "red", Color.Red);
    private static readonly PanelDetails _breakDetails = new("☕ Break", "blue", Color.Blue);

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

    private static TimerState RunIfNotExited(this TimerState state, Func<TimerState> op) =>
        state.Exit ? state : op();

    private static TimerState RunPomodoroPhase(IAnsiConsole console, TimerState state) =>
        state.Iter(_ => console.MarkupLines(
                            $"\n[bold green]🍅 Pomodoro Timer Started![/] Stay focused...",
                            $"Focus for [yellow]{state.PomodoroMinutes} minutes[/], starting now!",
                            "[dim](Press [yellow]P[/] to pause, [yellow]R[/] to resume, [yellow]Q[/] to quit)[/]\n"))
             .Map(s => RunTimerPhase(console, s, s.PomodoroMinutes * _minuteMultiplier, _pomodoroDetails)
                           .WithBeep(console, $"\n[bold green]✅ Pomodoro Complete! Time for a break.[/]")
                           .IncrementPomodoro());

    private static TimerState RunBreakPhase(IAnsiConsole console, TimerState state) =>
        state.Iter(_ => console.MarkupLine($"\n[bold blue]☕ Break Time! Relax and recharge...[/]"))
             .Map(s => RunTimerPhase(console, s, s.BreakMinutes * _minuteMultiplier, _breakDetails)
                           .WithBeep(console, $"\n[bold green]✅ Break is over! Back to work.[/]"));

    private static TimerState RunTimerPhase(
        IAnsiConsole console,
        TimerState state,
        int totalSeconds,
        PanelDetails details)
    {
        var panel = new TimerPanel(details);
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
            _ when console.Confirm("Are you ready to continue to the next cycle?") => state!,
            _ => state!.RequestExit()
        };
}
