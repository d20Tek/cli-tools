using D20Tek.Spectre.Console.Extensions.Settings;

namespace D20Tek.Tools.DevPassword.Commands;

internal sealed class PasswordSettings : VerbositySettings
{
    [CommandOption("-c|--count <COUNT>")]
    [Description("The number of GUIDs to generate (defaults to 1).")]
    [DefaultValue(1)]
    public int Count { get; set; } = 1;
}
