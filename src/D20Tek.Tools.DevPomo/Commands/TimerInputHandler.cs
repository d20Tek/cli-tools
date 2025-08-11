using Spectre.Console;

namespace D20Tek.Tools.DevPomo.Commands;

internal sealed class TimerInputHandler : IDisposable
{
    private readonly IAnsiConsole _console;
    private readonly TimerState _state;
    private Thread? _inputThread;
    private bool _running;

    public TimerInputHandler(IAnsiConsole console, TimerState state) => (_console, _state) = (console, state);

    public void Start()
    {
        _running = true;
        _inputThread = new Thread(ReadInput) { IsBackground = true };
        _inputThread.Start();
    }

    public static TimerInputHandler Start(IAnsiConsole console, TimerState state)
    {
        var handler = new TimerInputHandler(console, state);
        handler.Start();
        return handler;
    }

    private void ReadInput()
    {
        while (_running && !_state.Exit)
        {
            if (_console.Input.IsKeyAvailable())
            {
                var key = _console.Input.ReadKey(true)?.Key;
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
