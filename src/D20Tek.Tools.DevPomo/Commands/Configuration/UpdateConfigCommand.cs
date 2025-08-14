using D20Tek.Tools.DevPomo.Contracts;
using Spectre.Console;
using Spectre.Console.Cli;

namespace D20Tek.Tools.DevPomo.Commands.Configuration;

internal class UpdateConfigCommand : Command
{
    private readonly IAnsiConsole _console;
    private readonly IConfigurationService _service;

    public UpdateConfigCommand(IAnsiConsole console, IConfigurationService service) =>
        (_console, _service) = (console, service);

    public override int Execute(CommandContext context)
    {
        var prevConfig = _service.Get().GetValue();

        var pomoMinutes = _console.Prompt<int>(new TextPrompt<int>("Enter the pomodoro duration (in minutes)")
            .DefaultValue(prevConfig.PomodoroMinutes)
            .Validate(v => v >=5 && v <= 120, "Pomodoro duration must be between 5 and 120 minutes."));

        var breakMinutes = _console.Prompt<int>(new TextPrompt<int>("Enter the break duration (in minutes)")
            .DefaultValue(prevConfig.BreakMinutes)
            .Validate(v => v >= 1 && v <= 20, "Pomodoro break must be between 1 and 20 minutes."));

        var showAppTitle = _console.Confirm("Show the terminal application's title?", prevConfig.ShowAppTitleBar);
        var enableSound = _console.Confirm("Play notification sound on timer competion?", prevConfig.EnableSound);
        var autostartCycle = _console.Confirm("Auto-start new cycle when current one completes?", prevConfig.AutostartCycles);
        var minimalOutput = _console.Confirm("Show compact/minimal timer output?", prevConfig.MinimalOutput);

        var config = TimerConfiguration.Create(pomoMinutes, breakMinutes, showAppTitle, enableSound, autostartCycle, minimalOutput);

        var result = _service.Set(config);

        return 0;
    }
}
