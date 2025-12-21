using D20Tek.Spectre.Console.Extensions.Settings;

namespace D20Tek.Tools.DevPassword.Commands;

internal sealed class PasswordSettings : VerbositySettings
{
    [CommandOption("-c|--count <COUNT>")]
    [Description("The number of passwords to generate (defaults to 1).")]
    [DefaultValue(1)]
    public int Count { get; set; } = 1;

    [CommandOption("-l|--length <PASSWORD-LENGTH>")]
    [Description("The number of characters to use in the generated password (defaults to 25).")]
    [DefaultValue(Constants.DefaultPasswordLength)]
    public int Length { get; set; } = Constants.DefaultPasswordLength;
}
