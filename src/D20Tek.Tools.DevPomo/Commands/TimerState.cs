using System.Diagnostics;

namespace D20Tek.Tools.DevPomo.Commands;

internal sealed class TimerState
{
    public bool Paused { get; private set; } = false;

    public bool Exit { get; private set; } = false;

    public int PomodoroMinutes { get; } = 1;

    public int BreakMinutes { get; } = 1;

    public int CompletedPomodoro { get; private set; } = 0;

    private Stopwatch Stopwatch { get; } = new();

    public void Pause()
    {
        if (!Paused)
        {
            Paused = true;
            Stopwatch.Stop();
        }
    }

    public void Resume()
    {
        if (Paused)
        {
            Paused = false;
            Stopwatch.Start();
        }
    }

    public void RequestExit() => Exit = true;

    public void RestartTimer() => Stopwatch.Restart();

    public int GetElapsedSeconds() => (int)Stopwatch.Elapsed.TotalSeconds;

    public void IncrementPomodoro() => CompletedPomodoro++;
}
