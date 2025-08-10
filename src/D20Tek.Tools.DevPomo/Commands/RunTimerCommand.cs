using Spectre.Console;
using Spectre.Console.Cli;

namespace D20Tek.Tools.DevPomo.Commands;

internal class RunTimerCommand : Command
{
    private bool _paused = false;
    private bool _exit = false;

    public override int Execute(CommandContext context)
    {
        EmojiIcons.Initialize();

        const int pomodoroMinutes = 1; // change for testing
        var totalSeconds = pomodoroMinutes * 60;
        var endTime = DateTime.Now.AddMinutes(pomodoroMinutes);

        // Start background input thread
        var inputThread = new Thread(ReadInput) { IsBackground = true };
        inputThread.Start();

        AnsiConsole.Write(new FigletText("dev-pomo").Color(Color.Green));
        AnsiConsole.MarkupLine($"\n[bold green]{EmojiIcons.Tomato} Pomodoro Timer Started![/] Stay focused...");
        AnsiConsole.MarkupLine($"Focus for [yellow]{pomodoroMinutes} minutes[/], starting now!");
        AnsiConsole.MarkupLine("[dim](Press [yellow]P[/] to pause, [yellow]R[/] to resume, [yellow]Q[/] to quit)[/]\n");

        AnsiConsole.Live(new Panel(""))
                   .AutoClear(false)
                   .Overflow(VerticalOverflow.Ellipsis)
                   .Cropping(VerticalOverflowCropping.Top)
                   .Start(ctx =>
                    {
                        while (!_exit)
                        {
                            var remaining = endTime - DateTime.Now;
                            ctx.UpdateTarget(Render(remaining, totalSeconds, _paused));

                            if (remaining <= TimeSpan.Zero)
                                break;

                            Thread.Sleep(200);
                        }
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

        return 0;
    }

    private void ReadInput()
    {
        while (!_exit)
        {
            if (AnsiConsole.Console.Input.IsKeyAvailable())
            {
                var key = AnsiConsole.Console.Input.ReadKey(true)?.Key;
                if (key == ConsoleKey.P) _paused = true;
                if (key == ConsoleKey.R) _paused = false;
                if (key == ConsoleKey.Q) _exit = true;
            }

            Thread.Sleep(50);
        }
    }

    private Panel Render(TimeSpan remaining, int totalSeconds, bool paused)
    {
        double progressPercent = 1.0 - (remaining.TotalSeconds / totalSeconds);
        string timeLeft = $"{remaining.Minutes:D2}:{remaining.Seconds:D2}";

        string timeDisplay = paused ? $"[bold yellow]{timeLeft} - {EmojiIcons.Pause}  Paused[/]\n\n" : $"[bold red]{timeLeft}[/]\n\n";

        var panel = new Panel(
                timeDisplay +
                $"{GetProgressBar(progressPercent, 60)}\n\n" +
                "[dim]Commands: (P)ause (R)esume (Q)uit[/]")
            .Border(BoxBorder.Rounded)
            .BorderStyle(new Style(paused ? Color.Yellow : Color.Red))
            .Header($"{EmojiIcons.Tomato} Pomodoro", Justify.Center)
            .Padding(1, 1, 1, 1);

        return panel;
    }

    private string GetProgressBar(double percent, int width)
    {
        int filled = (int)(percent * width);
        int empty = width - filled;
        return $"[red]{new string('█', filled)}[/][grey]{new string('░', empty)}[/] {percent * 100:0}%";
    }
}
