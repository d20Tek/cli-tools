using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics;

namespace D20Tek.Tools.DevPomo.Commands;

internal class RunTimerCommand : Command
{
    private bool _paused = false;
    private bool _exit = false;
    private readonly Stopwatch _stopwatch = new();

    public override int Execute(CommandContext context)
    {
        EmojiIcons.Initialize();

        // Start background input thread
        var inputThread = new Thread(ReadInput) { IsBackground = true };
        inputThread.Start();

        const int pomodoroMinutes = 1; // change for testing

        AnsiConsole.Write(new FigletText("dev-pomo").Color(Color.Green));
        AnsiConsole.MarkupLine($"\n[bold green]{EmojiIcons.Tomato} Pomodoro Timer Started![/] Stay focused...");
        AnsiConsole.MarkupLine($"Focus for [yellow]{pomodoroMinutes} minutes[/], starting now!");
        AnsiConsole.MarkupLine("[dim](Press [yellow]P[/] to pause, [yellow]R[/] to resume, [yellow]Q[/] to quit)[/]\n");

        var totalSeconds = pomodoroMinutes * 60;
        var remainingSeconds = totalSeconds;

        _stopwatch.Start();

        AnsiConsole.Live(TimerPanel.Render(remainingSeconds, totalSeconds, _paused))
                   .AutoClear(false)
                   .Overflow(VerticalOverflow.Ellipsis)
                   .Cropping(VerticalOverflowCropping.Top)
                   .Start(ctx =>
                   {
                        while (!_exit && remainingSeconds > 0)
                        {
                            if (!_paused)
                            {
                                remainingSeconds = Math.Max(totalSeconds - (int)_stopwatch.Elapsed.TotalSeconds, 0);
                            }

                            ctx.UpdateTarget(TimerPanel.Render(remainingSeconds, totalSeconds, _paused));

                            Thread.Sleep(100);
                        }

                        _exit = true;
                   });

        if (!_exit)
        {
            Console.Beep();
            AnsiConsole.MarkupLine($"\n[bold green]Pomodoro Complete! Take a break.[/]");
        }
        else
        {
            AnsiConsole.MarkupLine($"\n[bold red]{EmojiIcons.Stop}  Pomodoro Stopped Early.[/]");
        }

        inputThread.Join();
        return 0;
    }

    private void ReadInput()
    {
        while (!_exit)
        {
            if (AnsiConsole.Console.Input.IsKeyAvailable())
            {
                var key = AnsiConsole.Console.Input.ReadKey(true)?.Key;
                switch (key)
                {
                    case ConsoleKey.P:
                        if (!_paused)
                        {
                            _paused = true;
                            _stopwatch.Stop();
                        }
                        break;
                    case ConsoleKey.R:
                        if (_paused)
                        {
                            _paused = false;
                            _stopwatch.Start();
                        }
                        break;
                    case ConsoleKey.Q:
                        _exit = true;
                        break;
                }
            }

            Thread.Sleep(50);
        }
    }
}
