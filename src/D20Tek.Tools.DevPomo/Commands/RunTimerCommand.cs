using Spectre.Console;
using Spectre.Console.Cli;

namespace D20Tek.Tools.DevPomo.Commands;

internal class RunTimerCommand : Command
{
    private readonly TimerState _state = new();

    public override int Execute(CommandContext context)
    {
        EmojiIcons.Initialize();
        using var inputHandler = TimerInputHandler.Start(_state);

        const int pomodoroMinutes = 1; // change for testing
        const int breakMinutes = 1;

        AnsiConsole.Write(new FigletText("dev-pomo").Color(Color.Green));

        RunPomodoroPhase(pomodoroMinutes);

        if (!_state.Exit)
        {
            RunBreakPhase(breakMinutes);
        }

        ShowEarlyExitMessage();
        return 0;
    }

    private void RunPomodoroPhase(int pomodoroMinutes)
    {
        AnsiConsole.MarkupLine($"\n[bold green]{EmojiIcons.Tomato} Pomodoro Timer Started![/] Stay focused...");
        AnsiConsole.MarkupLine($"Focus for [yellow]{pomodoroMinutes} minutes[/], starting now!");
        AnsiConsole.MarkupLine("[dim](Press [yellow]P[/] to pause, [yellow]R[/] to resume, [yellow]Q[/] to quit)[/]\n");

        var totalSeconds = pomodoroMinutes * 60;
        RunTimerPhase(totalSeconds, $"{EmojiIcons.Tomato} Pomodoro");

        if (!_state.Exit)
        {
            Console.Beep();
            AnsiConsole.MarkupLine($"\n[bold green]Pomodoro Complete! Time for a break.[/]");
        }
    }

    private void RunBreakPhase(int minutes)
    {
        AnsiConsole.MarkupLine($"\n[bold blue]Break Time! Relax and recharge...[/]");
        RunTimerPhase(minutes * 60, $"{EmojiIcons.Coffee} Break", "blue", Color.Blue);

        if (!_state.Exit)
        {
            Console.Beep();
            AnsiConsole.MarkupLine($"\n[bold green]Break is over! Back to work.[/]");
        }
    }

    private void RunTimerPhase(int totalSeconds, string title, string progressColor = "red", Color? borderColor = null)
    {
        int remainingSeconds = totalSeconds;
        var panel = new TimerPanel(title, progressColor, borderColor);

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
            AnsiConsole.MarkupLine($"\n[bold red]{EmojiIcons.Stop}  Pomodoro Stopped Early.[/]");
        }
    }
}
