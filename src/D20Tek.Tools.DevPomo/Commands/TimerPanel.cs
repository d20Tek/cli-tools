using Spectre.Console;

namespace D20Tek.Tools.DevPomo.Commands;

internal sealed class TimerPanel
{
    private readonly string _title;
    private readonly string _foregroundColor;
    private readonly Color _borderColor;

    public TimerPanel(string title, string foregroundColor = "red", Color? borderColor = null)
    {
        _title = title;
        _foregroundColor = foregroundColor;
        _borderColor = borderColor ?? Color.Red;
    }

    public TimerPanel(PanelDetails details)
        : this(details.Title, details.ForegroundColor, details.BorderColor) { }

    public Panel Render(int remainingSeconds, int totalSeconds, bool paused)
    {
        double progressPercent = (double)(totalSeconds - remainingSeconds) / totalSeconds;
        string timeLeft = FormatTime(remainingSeconds);

        var panel = new Panel(
                RenderTime(timeLeft, _foregroundColor, paused) +
                $"{RenderProgressBar(progressPercent, 60, paused, _foregroundColor)}\n\n" +
                "[dim]Commands: (P)ause (R)esume (Q)uit[/]")
            .Border(BoxBorder.Rounded)
            .BorderStyle(new Style(paused ? Color.Yellow : _borderColor))
            .Header(_title, Justify.Center)
            .Padding(1, 1, 1, 1);

        return panel;
    }

    private static string FormatTime(int remainingSeconds) =>
        $"{(remainingSeconds / 60):D2}:{(remainingSeconds % 60):D2}";

    private static string RenderTime(string timeLeft, string color, bool paused) =>
        paused ? $"[bold yellow]{timeLeft} - ⏸  Paused[/]\n\n" : $"[bold {color}]{timeLeft}[/]\n\n";

    private static string RenderProgressBar(
        double percent,
        int width,
        bool paused,
        string foregroundColor,
        string backgroundColor = "grey")
    {
        string foreStyle = paused ? $"{foregroundColor} dim" : foregroundColor;
        string backStyle = paused ? $"{backgroundColor} dim" : backgroundColor;
        int filled = (int)(percent * width);
        int empty = width - filled;

        return $"[{foreStyle}]{new string('█', filled)}[/][{backStyle}]{new string('░', empty)}[/]" +
               $" {percent * 100,3:0}%";
    }
}

internal record PanelDetails(string Title, string ForegroundColor, Color BorderColor);
