using System.Diagnostics;

namespace D20Tek.Tools.DevPomo.Commands;

internal sealed class TimerState
{
    private readonly Stopwatch _stopwatch = new();

    public bool Paused { get; private set; } = false;

    public bool Exit { get; private set; } = false;

    public int PomodoroMinutes { get; } = 1;

    public int BreakMinutes { get; } = 1;

    public int PomodorosToRun { get; private set;} = 4;

    public int CompletedPomodoro { get; private set; } = 0;

    public void Pause()
    {
        if (!Paused)
        {
            Paused = true;
            _stopwatch.Stop();
        }
    }

    public void Resume()
    {
        if (Paused)
        {
            Paused = false;
            _stopwatch.Start();
        }
    }

    public void RequestExit() => Exit = true;

    public void RestartTimer() => _stopwatch.Restart();

    public int GetElapsedSeconds() => (int)_stopwatch.Elapsed.TotalSeconds;

    public void IncrementPomodoro() => CompletedPomodoro++;

    public void SetPomodorosToRun(int count)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(count, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(count, 50);
        PomodorosToRun = count;
    }

    public bool ArePomodorosComplete() => CompletedPomodoro == PomodorosToRun;
}
