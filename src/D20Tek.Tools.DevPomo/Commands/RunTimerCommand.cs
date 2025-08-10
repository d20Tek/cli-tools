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

        AnsiConsole.Write(new FigletText("dev-pomo").Color(Color.Green));
        AnsiConsole.MarkupLine($"\n[bold green]{EmojiIcons.Tomato} Pomodoro Timer Started![/] Stay focused...");
        AnsiConsole.MarkupLine($"Focus for [yellow]{pomodoroMinutes} minutes[/], starting now!");
        AnsiConsole.MarkupLine("[dim](Press [yellow]P[/] to pause, [yellow]R[/] to resume, [yellow]Q[/] to quit)[/]\n");

        var totalSeconds = pomodoroMinutes * 60;
        var remainingSeconds = totalSeconds;

        _state.Stopwatch.Start();

        AnsiConsole.Live(TimerPanel.Render(remainingSeconds, totalSeconds, _state.Paused))
                   .AutoClear(false)
                   .Overflow(VerticalOverflow.Ellipsis)
                   .Cropping(VerticalOverflowCropping.Top)
                   .Start(ctx =>
                   {
                        while (!_state.Exit && remainingSeconds > 0)
                        {
                            if (!_state.Paused)
                            {
                                remainingSeconds = Math.Max(totalSeconds - (int)_state.Stopwatch.Elapsed.TotalSeconds, 0);
                            }

                            ctx.UpdateTarget(TimerPanel.Render(remainingSeconds, totalSeconds, _state.Paused));

                            Thread.Sleep(100);
                        }
                   });

        ShowEndMessage(remainingSeconds);

        return 0;
    }

    private void ShowEndMessage(int remainingSeconds)
    {
        if (remainingSeconds <= 0)
        {
            Console.Beep();
            AnsiConsole.MarkupLine($"\n[bold green]Pomodoro Complete! Take a break.[/]");
        }
        else
        {
            AnsiConsole.MarkupLine($"\n[bold red]{EmojiIcons.Stop}  Pomodoro Stopped Early.[/]");
        }
    }
}
