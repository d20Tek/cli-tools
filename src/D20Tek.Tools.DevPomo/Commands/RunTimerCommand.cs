using Spectre.Console;
using Spectre.Console.Cli;
using D20Tek.Tools.DevPomo.Common;

namespace D20Tek.Tools.DevPomo.Commands;

internal sealed class RunTimerCommand : Command
{
    private const int _minuteMultiplier = 10;
    private readonly TimerState _state = new();

    public override int Execute(CommandContext context)
    {
        ConsoleExtensions.SupportsEmoji();
        using var inputHandler = TimerInputHandler.Start(_state);

        AnsiConsole.Console.DisplayAppHeader("dev-pomo");

        RunPomodoroPhase(_state.PomodoroMinutes);

        if (!_state.Exit)
        {
            RunBreakPhase(_state.BreakMinutes);
        }

        ShowExitMessage();
        return 0;
    }

    private void RunPomodoroPhase(int pomodoroMinutes)
    {
        AnsiConsole.Console.MarkupLines([ 
            $"\n[bold green]🍅 Pomodoro Timer Started![/] Stay focused...",
            $"Focus for [yellow]{pomodoroMinutes} minutes[/], starting now!",
            "[dim](Press [yellow]P[/] to pause, [yellow]R[/] to resume, [yellow]Q[/] to quit)[/]\n"]);

        RunTimerPhase(pomodoroMinutes * _minuteMultiplier, $"🍅 Pomodoro", "red", Color.Red);

        if (!_state.Exit)
        {
            Console.Beep();
            AnsiConsole.MarkupLine($"\n[bold green]✅ Pomodoro Complete! Time for a break.[/]");
            _state.IncrementPomodoro();
        }
    }

    private void RunBreakPhase(int minutes)
    {
        AnsiConsole.MarkupLine($"\n[bold blue]☕ Break Time! Relax and recharge...[/]");
        RunTimerPhase(minutes * _minuteMultiplier, $"☕ Break", "blue", Color.Blue);

        if (!_state.Exit)
        {
            Console.Beep();
            AnsiConsole.MarkupLine($"\n[bold green]✅ Break is over! Back to work.[/]");
        }
    }

    private void RunTimerPhase(int totalSeconds, string title, string foregroundColor, Color borderColor)
    {
        int remainingSeconds = totalSeconds;
        var panel = new TimerPanel(title, foregroundColor, borderColor);

        _state.RestartTimer();

        AnsiConsole.Live(panel.Render(remainingSeconds, totalSeconds, _state.Paused))
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

    private void ShowExitMessage()
    {
        if (_state.Exit)
        {
            AnsiConsole.MarkupLine($"\n[bold red]⏹  Pomodoro Stopped Early.[/]");
            AnsiConsole.Console.MarkupLinesConditional(
                _state.CompletedPomodoro > 0,
                $"But you completed {_state.CompletedPomodoro} pomodoro(s) before stopping.");
        }
        else
        {
            AnsiConsole.MarkupLine(
                $"\n[bold green]Pomodoro run ended! You completed {_state.CompletedPomodoro} pomodoro(s).[/]");
        }
    }
}
