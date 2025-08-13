namespace D20Tek.Tools.DevPomo.Contracts;

public record TimerConfiguration(
    int PomodoroMinutes,
    int BreakMinutes,
    bool ShowAppTitleBar,
    bool EnableSound,
    bool AutostartCycles,
    bool MinimalOutput)
{
    public TimerConfiguration()
        : this(25, 5, true, true, false, false) { }
}
