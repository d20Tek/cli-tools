using Spectre.Console;

namespace D20Tek.Tools.DevPomo.Commands;

internal static class TimerPanel
{
    public static Panel Render(int remainingSeconds, int totalSeconds, bool paused)
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
