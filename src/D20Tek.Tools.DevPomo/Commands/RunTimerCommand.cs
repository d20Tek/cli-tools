using Spectre.Console;
using Spectre.Console.Cli;

namespace D20Tek.Tools.DevPomo.Commands;

internal class RunTimerCommand : Command
{
    private bool _paused = false;
    private bool _exit = false;
    private bool _emojiSupported;

    public override int Execute(CommandContext context)
    {
        _emojiSupported = DetectEmojiSupport();

        const int pomodoroMinutes = 25; // change for testing
        var totalSeconds = pomodoroMinutes * 60;
        var endTime = DateTime.Now.AddMinutes(pomodoroMinutes);

        // Start background input thread
        var inputThread = new Thread(ReadInput) { IsBackground = true };
        inputThread.Start();

        AnsiConsole.Write(new FigletText("dev-pomo").Color(Color.Green));
        AnsiConsole.MarkupLine($"\n[bold green]{IconTomato()} Pomodoro Timer Started![/] Stay focused...");
        AnsiConsole.MarkupLine($"Focus for [yellow]{pomodoroMinutes} minutes[/], starting now!");
        AnsiConsole.MarkupLine("[dim](Press [yellow]P[/] to pause, [yellow]R[/] to resume, [yellow]Q[/] to quit)[/]\n");

        AnsiConsole.Live(new Panel("")).Start(ctx =>
        {
            while (!_exit)
            {
                if (!_paused)
                {
                    var remaining = endTime - DateTime.Now;
                    if (remaining <= TimeSpan.Zero)
                        break;

                    double progressPercent = 1.0 - (remaining.TotalSeconds / totalSeconds);
                    string timeLeft = $"{remaining.Minutes:D2}:{remaining.Seconds:D2}";

                    var panel = new Panel(
                        $"[bold red]{timeLeft}[/]\n\n" +
                        $"{GetProgressBar(progressPercent, 60)}\n\n" +
                        "[dim]Commands: (P)ause (Q)uit[/]")
                        .Border(BoxBorder.Rounded)
                        .BorderStyle(new Style(Color.Red))
                        .Header($"{IconTomato()} Pomodoro", Justify.Center)
                        .Padding(1, 1, 1, 1);

                    ctx.UpdateTarget(panel);
                }
                else
                {
                    var panel = new Panel($"{IconPause()}  [yellow]Paused[/]\n\n[dim](R)esume  (Q)uit[/]")
                        .Border(BoxBorder.Rounded)
                        .BorderStyle(new Style(Color.Yellow))
                        .Header($"{IconTomato()}  Pomodoro", Justify.Center)
                        .Padding(1, 1, 1, 1);

                    ctx.UpdateTarget(panel);
                }

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
            AnsiConsole.MarkupLine($"\n[bold red]{IconStop()}  Pomodoro Stopped Early.[/]");
        }

        return 0;
    }

    private void ReadInput()
    {
        while (!_exit)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.P) _paused = true;
                if (key == ConsoleKey.R) _paused = false;
                if (key == ConsoleKey.Q) _exit = true;
            }
            Thread.Sleep(50); // prevent CPU overuse
        }
    }

    private string GetProgressBar(double percent, int width)
    {
        int filled = (int)(percent * width);
        int empty = width - filled;
        return $"[red]{new string('█', filled)}[/][grey]{new string('░', empty)}[/] {percent * 100:0}%";
    }

    // Emoji / ASCII icon helpers
    private string IconTomato() => _emojiSupported ? ":tomato:" : "[red]*[/]";

    private string IconPause() => _emojiSupported ? ":pause_button:" : "[yellow]||[/]";

    private string IconStop() => _emojiSupported ? ":stop_button:" : "[red]STOP[/]";

    // Simple emoji detection
    private bool DetectEmojiSupport()
    {
        // Check if console can render a known emoji character
        try
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            string test = "🍅";
            return test.Any(c => char.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.OtherNotAssigned);
        }
        catch
        {
            return false;
        }
    }
}
