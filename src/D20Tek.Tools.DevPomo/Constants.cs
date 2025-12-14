using D20Tek.Tools.DevPomo.Commands.RunTimer;

namespace D20Tek.Tools.DevPomo;

internal static class Constants
{
    public const string AppTitle = "dev-pomo";
    public const int DefaultPomodoroMinutes = 25;
    public const int DefaultRestMinutes = 5;
    public const int DefaultNumCycles = 4;

    public static class Configuration
    {
        public const string Description = "Update the configuration for the pomodoro timers.";
        public const string PomodoroMinutesLabel = "Enter the pomodoro duration (in minutes)";
        public const string PomodoroMinutesError = "Pomodoro duration must be between 5 and 120 minutes.";
        public const string BreakMinutesLabel = "Enter the break duration (in minutes)";
        public const string BreakMinutesError = "Pomodoro break must be between 1 and 20 minutes.";
        public const string ShowAppTitleLabel = "Show the terminal application's title?";
        public const string PlaySoundLabel = "Play notification sound on timer competion?";
        public const string AutoStartCyclesLabel = "Auto-start new cycle when current one completes?";
        public const string ShowMinimalOutputLabel = "Show compact/minimal timer output?";
    }

    public static class Timer
    {
        public const int MinuteMultiplier = 60;
        public const string DefaultForegroundColor = "red";
        public const string DefaultProgressBarBackgoundColor = "grey";
        public const string TimerPanelCommands = "\n[dim]Commands: (P)ause (R)esume (Q)uit[/]";
        public static string FormatTime(int seconds) => $"{seconds / 60:D2}:{seconds % 60:D2}";
        public static string RenderTime(string timeLeft, string color, bool paused) =>
            paused ? $"[bold yellow]{timeLeft} - ⏸  Paused[/]\n\n" : $"[bold {color}]{timeLeft}[/]\n\n";
        public static int GetWidth(bool minimalOutput) => minimalOutput ? 40 : 60;

        public static readonly PanelDetails PomodoroDetails = new("🍅 Pomodoro", "red", Color.Red);
        public static readonly PanelDetails BreakDetails = new("☕ Break", "blue", Color.Blue);
        public const string EndPomoCycleMsg = $"\n[bold green]✅ Pomodoro Complete! Time for a break.[/]";
        public const string PomoStartedMsg = "\n[bold green]🍅 Pomodoro Timer Started![/] Stay focused...";
        public static string[] PomoStartedFullMsg(int pomodoroMinutes) =>
        [
            $"Focus for [yellow]{pomodoroMinutes} minutes[/], starting now!",
            "[dim](Press [yellow]P[/] to pause, [yellow]R[/] to resume, [yellow]Q[/] to quit)[/]\n"
        ];
        public const string StartBreakMsg = "\n[bold blue]☕ Break Time! Relax and recharge...[/]";
        public const string EndBreakMsg = "\n[bold green]✅ Break is over! Back to work.[/]";
        public const string ContinueCyclesLabel = "Are you ready to continue to the next cycle?";

        public const string EarlyStopMsg = "\n[bold red]⏹  Pomodoro Stopped Early.[/]";
        public static string EarlyStopPomoCount(int completedPomodoros) =>
            $"But you completed {completedPomodoros} pomodoro(s) before stopping.";
        public static string CompletedMsg(int completedPomodoros) =>
            $"\n[bold green]Pomodoro run ended! You completed {completedPomodoros} pomodoro(s).[/]";
    }
}
