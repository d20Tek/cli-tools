using Spectre.Console;
using Spectre.Console.Cli;
using D20Tek.Tools.DevPomo.Common;

namespace D20Tek.Tools.DevPomo.Commands;

internal class RunTimerCommand : Command
{
    private const int _minuteMultiplier = 10;
    private readonly TimerState _state = new();

    public override int Execute(CommandContext context)
    {
        ConsoleExtensions.SupportsEmoji();
        using var inputHandler = TimerInputHandler.Start(_state);

        AnsiConsole.Write(new FigletText("dev-pomo").Color(Color.Green));

        RunPomodoroPhase(_state.PomodoroMinutes);

        if (!_state.Exit)
        {
            RunBreakPhase(_state.BreakMinutes);
        }

        ShowEarlyExitMessage();
        return 0;
    }

    private void RunPomodoroPhase(int pomodoroMinutes)
    {
        AnsiConsole.MarkupLine($"\n[bold green]🍅 Pomodoro Timer Started![/] Stay focused...");
        AnsiConsole.MarkupLine($"Focus for [yellow]{pomodoroMinutes} minutes[/], starting now!");
        AnsiConsole.MarkupLine("[dim](Press [yellow]P[/] to pause, [yellow]R[/] to resume, [yellow]Q[/] to quit)[/]\n");

        RunTimerPhase(pomodoroMinutes * _minuteMultiplier, $"🍅 Pomodoro", "red", Color.Red);

        if (!_state.Exit)
        {
            Console.Beep();
            AnsiConsole.MarkupLine($"\n[bold green]✅ Pomodoro Complete! Time for a break.[/]");
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
                               remainingSeconds = Math.Max(totalSeconds - _state.GetElapsedTime(), 0);
                           }

                           ctx.UpdateTarget(panel.Render(remainingSeconds, totalSeconds, _state.Paused));

                           Thread.Sleep(100);
                       }
                   });
    }

    private void ShowEarlyExitMessage()
    {
        if (_state.Exit)
        {
            AnsiConsole.MarkupLine($"\n[bold red]⏹  Pomodoro Stopped Early.[/]");
        }
    }
}
