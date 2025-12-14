using System.Text.Json.Serialization;

namespace D20Tek.Tools.DevPomo.Contracts;

public class TimerConfiguration
{
    public int PomodoroMinutes { get; private set; } = Constants.DefaultPomodoroMinutes;

    public int BreakMinutes { get; private set; } = Constants.DefaultRestMinutes;

    public bool ShowAppTitleBar { get; private set; } = true;

    public bool EnableSound { get; private set; } = true;

    public bool AutostartCycles { get; private set; } = false;

    public bool MinimalOutput { get; private set; } = false;

    public TimerConfiguration() { }

    [JsonConstructor]
    public TimerConfiguration(
        int pomodoroMinutes,
        int breakMinutes,
        bool showAppTitleBar,
        bool enableSound,
        bool autostartCycles,
        bool minimalOutput)
    {
        PomodoroMinutes = pomodoroMinutes;
        BreakMinutes = breakMinutes;
        ShowAppTitleBar = showAppTitleBar;
        EnableSound = enableSound;
        AutostartCycles = autostartCycles;
        MinimalOutput = minimalOutput;
    }

    public static TimerConfiguration Create(
        int pomoMinutes,
        int breakMinutes,
        bool showAppTitle,
        bool enableSound,
        bool autostartCycles,
        bool minimalOutput) =>
        new(pomoMinutes, breakMinutes, showAppTitle, enableSound, autostartCycles, minimalOutput);

    public void Update(
        int pomoMinutes,
        int breakMinutes,
        bool showAppTitle,
        bool enableSound,
        bool autostartCycles,
        bool minimalOutput)
    {
        PomodoroMinutes = pomoMinutes;
        BreakMinutes = breakMinutes;
        ShowAppTitleBar = showAppTitle;
        EnableSound = enableSound;
        AutostartCycles = autostartCycles;
        MinimalOutput = minimalOutput;
    }
}
