using System.Diagnostics;

namespace D20Tek.Tools.DevPomo.Commands.RunTimer;

internal sealed class TimerState : IState
{
    private readonly Stopwatch _stopwatch = new();

    public bool Paused { get; private set; } = false;

    public bool Exit { get; private set; } = false;

    public int PomodoroMinutes { get; private set; } = Constants.DefaultPomodoroMinutes;

    public int BreakMinutes { get; private set; } = Constants.DefaultRestMinutes;

    public int PomodoroCycles { get; private set;} = Constants.DefaultNumCycles;

    public int CompletedPomodoros { get; private set; } = 0;

    public TimerConfiguration Configuration = new();

    public static TimerState Create(TimerConfiguration config) => new()
    {
        PomodoroMinutes = config.PomodoroMinutes,
        BreakMinutes = config.BreakMinutes,
        Configuration = config
    };

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

    public TimerState RequestExit()
    {
        Exit = true;
        return this;
    }

    public TimerState RestartTimer()
    {
        _stopwatch.Restart();
        return this;
    }

    public int GetElapsedSeconds() => (int)_stopwatch.Elapsed.TotalSeconds;

    public TimerState IncrementPomodoro()
    {
        if (!Exit)
        {
            CompletedPomodoros++;
        }
        return this;
    }

    public TimerState SetPomodoroCycles(int count)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(count, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(count, 50);
        PomodoroCycles = count;
        return this;
    }

    public bool ArePomodorosComplete() => CompletedPomodoros >= PomodoroCycles;

    public TimerState WithBeep(IAnsiConsole console, string message)
    {
        if (!Exit)
        {
            if (Configuration.EnableSound) console.Beep();
            console.MarkupLine(message);
        }
        return this;
    }
}
