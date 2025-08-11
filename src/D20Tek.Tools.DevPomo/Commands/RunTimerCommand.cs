using Spectre.Console;
using Spectre.Console.Cli;
using D20Tek.Tools.DevPomo.Common;

namespace D20Tek.Tools.DevPomo.Commands;

internal sealed class RunTimerCommand : Command
{
    private readonly TimerState _state = new();
    private readonly IAnsiConsole _console;

    public RunTimerCommand(IAnsiConsole console) => _console = console;

    public override int Execute(CommandContext context)
    {
        _console.DisplayAppHeader("dev-pomo", Justify.Left);
        using var inputHandler = TimerInputHandler.Start(_console, _state);

        PomodoroEngine.Initialize(_console, _state).Run();
        ShowExitMessage();

        return 0;
    }

    private void ShowExitMessage()
    {
        if (_state.Exit)
        {
            _console.MarkupLine($"\n[bold red]⏹  Pomodoro Stopped Early.[/]");
            _console.MarkupLinesConditional(
                _state.CompletedPomodoro > 0,
                $"But you completed {_state.CompletedPomodoro} pomodoro(s) before stopping.");
        }
        else
        {
            _console.MarkupLine(
                $"\n[bold green]Pomodoro run ended! You completed {_state.CompletedPomodoro} pomodoro(s).[/]");
        }
    }
}
