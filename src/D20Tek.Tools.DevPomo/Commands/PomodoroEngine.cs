using D20Tek.Tools.DevPomo.Common;
using Spectre.Console;

namespace D20Tek.Tools.DevPomo.Commands;

internal sealed class PomodoroEngine
{
    private const int _minuteMultiplier = 10;
    private readonly IAnsiConsole _console;
    private readonly TimerState _state;

    private PomodoroEngine(IAnsiConsole console, TimerState state) =>
        (_console, _state) = (console, state);

    public static PomodoroEngine Initialize(IAnsiConsole console, TimerState state) => new(console, state);

    public void Run()
    {
        for (int i = 0; i < _state.PomodorosToRun; i++)
        {
            RunPomodoroPhase(_state.PomodoroMinutes);
            if (_state.Exit) break;

            RunBreakPhase(_state.BreakMinutes);
            if (_state.Exit) break;

            ConfirmRestart();
            if (_state.Exit) break;
        }
    }

    private void RunPomodoroPhase(int pomodoroMinutes)
    {
        _console.MarkupLines([
            $"\n[bold green]🍅 Pomodoro Timer Started![/] Stay focused...",
            $"Focus for [yellow]{pomodoroMinutes} minutes[/], starting now!",
            "[dim](Press [yellow]P[/] to pause, [yellow]R[/] to resume, [yellow]Q[/] to quit)[/]\n"]);

        RunTimerPhase(pomodoroMinutes * _minuteMultiplier, $"🍅 Pomodoro", "red", Color.Red);

        if (!_state.Exit)
        {
            _console.Beep();
            _console.MarkupLine($"\n[bold green]✅ Pomodoro Complete! Time for a break.[/]");
            _state.IncrementPomodoro();
        }
    }

    private void RunBreakPhase(int minutes)
    {
        _console.MarkupLine($"\n[bold blue]☕ Break Time! Relax and recharge...[/]");
        RunTimerPhase(minutes * _minuteMultiplier, $"☕ Break", "blue", Color.Blue);

        if (!_state.Exit)
        {
            _console.Beep();
            _console.MarkupLine($"\n[bold green]✅ Break is over! Back to work.[/]");
        }
    }

    private void RunTimerPhase(int totalSeconds, string title, string foregroundColor, Color borderColor)
    {
        int remainingSeconds = totalSeconds;
        var panel = new TimerPanel(title, foregroundColor, borderColor);

        _state.RestartTimer();

        _console.Live(panel.Render(remainingSeconds, totalSeconds, _state.Paused))
                .AutoClear(false)
                .Overflow(VerticalOverflow.Ellipsis)
                .Cropping(VerticalOverflowCropping.Top)
                .Start(ctx =>
                {
                    while (!_state.Exit && remainingSeconds > 0)
                    {
                        if (!_state.Paused)
                        {
                            remainingSeconds = Math.Max(totalSeconds - _state.GetElapsedSeconds(), 0);
                        }

                        ctx.UpdateTarget(panel.Render(remainingSeconds, totalSeconds, _state.Paused));

                        Thread.Sleep(100);
                    }
                });
    }

    private void ConfirmRestart()
    {
        if (!_state.ArePomodorosComplete())
        {
            bool restart = _console.Confirm("Are you ready to continue to the next cycle?");
            if (!restart)
            {
                _state.RequestExit();
            }
        }
    }
}
