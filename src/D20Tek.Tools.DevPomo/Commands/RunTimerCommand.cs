using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics;

namespace D20Tek.Tools.DevPomo.Commands;

internal class RunTimerCommand : Command
{
    private bool _paused = false;
    private bool _exit = false;
    private Stopwatch _stopwatch = new();

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

        AnsiConsole.Live(Render(remainingSeconds, totalSeconds, _paused))
                   .AutoClear(false)
                   .Overflow(VerticalOverflow.Ellipsis)
                   .Cropping(VerticalOverflowCropping.Top)
                   .Start(ctx =>
                   {
                        while (!_exit && remainingSeconds > 0)
                        {
                            remainingSeconds = totalSeconds - (int)_stopwatch.Elapsed.TotalSeconds;
                            ctx.UpdateTarget(Render(remainingSeconds, totalSeconds, _paused));

                            Thread.Sleep(100);
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

    private static Panel Render(int remainingSeconds, int totalSeconds, bool paused)
    {
        double progressPercent = (double)(totalSeconds - remainingSeconds) / totalSeconds;
        string timeLeft = FormatTime(remainingSeconds);

        var panel = new Panel(
                RenderTime(timeLeft, paused) +
                $"{RenderProgressBar(progressPercent, 60)}\n\n" +
                "[dim]Commands: (P)ause (R)esume (Q)uit[/]")
            .Border(BoxBorder.Rounded)
            .BorderStyle(new Style(paused ? Color.Yellow : Color.Red))
            .Header($"{EmojiIcons.Tomato} Pomodoro", Justify.Center)
            .Padding(1, 1, 1, 1);

        return panel;
    }

    private static string FormatTime(int remainingSeconds) =>
        $"{(remainingSeconds / 60):D2}:{(remainingSeconds % 60):D2}";

    private static string RenderTime(string timeLeft, bool paused) =>
        paused ? $"[bold yellow]{timeLeft} - {EmojiIcons.Pause}  Paused[/]\n\n" : $"[bold red]{timeLeft}[/]\n\n";

    private static string RenderProgressBar(
        double percent,
        int width,
        string foregroundColor = "red",
        string backgroundColor = "grey")
    {
        int filled = (int)(percent * width);
        int empty = width - filled;
        return $"[{foregroundColor}]{new string('█', filled)}[/][{backgroundColor}]{new string('░', empty)}[/]" +
               $" {percent * 100:0}%";
    }
}
