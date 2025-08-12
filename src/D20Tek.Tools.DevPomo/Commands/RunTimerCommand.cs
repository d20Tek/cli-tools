using D20Tek.Functional;
using D20Tek.Tools.DevPomo.Common;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace D20Tek.Tools.DevPomo.Commands;

internal sealed class RunTimerCommand : Command<RunTimerCommand.Settings>
{
    internal class Settings : CommandSettings
    {
        [CommandOption("-c|--cycles <POMODORO-CYCLES>")]
        [Description("Defines how many iterations of pomodoros to run in a session (defaults to 4).")]
        [DefaultValue(4)]
        public int Cycles { get; set; }
    }

    private readonly IAnsiConsole _console;

    public RunTimerCommand(IAnsiConsole console) => _console = console;

    public override int Execute(CommandContext context, Settings settings)
    {
        _console.DisplayAppHeader("dev-pomo", Justify.Left);
        var state = TimerState.Create();
        using var inputHandler = TimerInputHandler.Start(_console, state);

        state.Iter(s => s.SetPomodoroCycles(settings.Cycles))
             .Map(s => PomodoroEngine.Run(_console, s))
             .Iter(s => ShowExitMessage(_console, s));

        return 0;
    }

    private static void ShowExitMessage(IAnsiConsole console, TimerState state)
    {
        if (state.Exit)
        {
            console.MarkupLine($"\n[bold red]⏹  Pomodoro Stopped Early.[/]");
            console.MarkupLinesConditional(
                state.CompletedPomodoro > 0,
                $"But you completed {state.CompletedPomodoro} pomodoro(s) before stopping.");
        }
        else
        {
            console.MarkupLine(
                $"\n[bold green]Pomodoro run ended! You completed {state.CompletedPomodoro} pomodoro(s).[/]");
        }
    }
}
