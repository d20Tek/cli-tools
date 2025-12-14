namespace D20Tek.Tools.DevPomo.Commands.RunTimer;

internal sealed class TimerPanel(
    string title,
    bool minimalOutput,
    string foregroundColor = Constants.Timer.DefaultForegroundColor,
    Color? borderColor = null)
{
    private readonly string _title = title;
    private readonly bool _minimalOutput = minimalOutput;
    private readonly string _foregroundColor = foregroundColor;
    private readonly Color _borderColor = borderColor ?? Color.Red;

    public TimerPanel(PanelDetails details, bool minimalOutput) :
        this(details.Title, minimalOutput, details.ForegroundColor, details.BorderColor) { }

    public Panel Render(int remainingSeconds, int totalSeconds, bool paused)
    {
        var progressPercent = (double)(totalSeconds - remainingSeconds) / totalSeconds;
        var timeLeft = Constants.Timer.FormatTime(remainingSeconds);
        var width = Constants.Timer.GetWidth(_minimalOutput);

        return new Panel(
                Constants.Timer.RenderTime(timeLeft, _foregroundColor, paused) +
                $"{RenderProgressBar(progressPercent, width, paused, _foregroundColor)}\n" +
                GetCommandsDisplay())
            .Border(BoxBorder.Rounded)
            .BorderStyle(new Style(paused ? Color.Yellow : _borderColor))
            .Header(_title, Justify.Center)
            .Padding(1, 1, 1, 1);
    }

    private string GetCommandsDisplay() => _minimalOutput ? string.Empty : Constants.Timer.TimerPanelCommands;

    private static string RenderProgressBar(
        double percent,
        int width,
        bool paused,
        string foregroundColor,
        string backgroundColor = Constants.Timer.DefaultProgressBarBackgoundColor)
    {
        var foreStyle = paused ? $"{foregroundColor} dim" : foregroundColor;
        var backStyle = paused ? $"{backgroundColor} dim" : backgroundColor;
        var filled = (int)(percent * width);
        var empty = width - filled;

        return $"[{foreStyle}]{new string('█', filled)}[/][{backStyle}]{new string('░', empty)}[/]" +
               $" {percent * 100,3:0}%";
    }
}

internal record PanelDetails(string Title, string ForegroundColor, Color BorderColor);
