using Spectre.Console;

namespace D20Tek.Tools.DevPomo.Commands;

internal sealed class TimerInputHandler : IDisposable
{
    private readonly TimerState _state;
    private Thread? _inputThread;
    private bool _running;

    public TimerInputHandler(TimerState state) => _state = state;

    public void Start()
    {
        _running = true;
        _inputThread = new Thread(ReadInput) { IsBackground = true };
        _inputThread.Start();
    }

    public static TimerInputHandler Start(TimerState state)
    {
        var handler = new TimerInputHandler(state);
        handler.Start();
        return handler;
    }

    private void ReadInput()
    {
        while (_running && !_state.Exit)
        {
            if (AnsiConsole.Console.Input.IsKeyAvailable())
            {
                var key = AnsiConsole.Console.Input.ReadKey(true)?.Key;
                if (key == ConsoleKey.P) _state.Pause();
                else if (key == ConsoleKey.R) _state.Resume();
                else if (key == ConsoleKey.Q) _state.RequestExit();
            }

            Thread.Sleep(50);
        }
    }

    public void Dispose()
    {
        _running = false;
        _inputThread?.Join();
    }
}
