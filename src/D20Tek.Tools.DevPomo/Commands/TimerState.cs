using System.Diagnostics;

namespace D20Tek.Tools.DevPomo.Commands;

internal class TimerState
{
    public bool Paused { get; private set; } = false;

    public bool Exit { get; private set; } = false;

    public Stopwatch Stopwatch { get; } = new();

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

    public void RequestExit()
    {
        Exit = true;
    }
}
