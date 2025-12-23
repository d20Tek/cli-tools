namespace D20Tek.Tools.DevPassword;

internal static class Constants
{
    public const string AppName = "dev-password";
    public const string ConfigFile = "password.config";
    public const string DataFolder = "data";

    public const int DefaultPasswordLength = 25;
    public const int MinPasswordLength = 4;
    public const int MaxPasswordLength = 64;
    public const int CountMin = 1;
    public const int CountMax = 100;

    public static string PasswordMessage(string password) =>
        $"[yellow]Generated Password:[/] {password.EscapeMarkup()}";

    public static string EntropyMessage(double entropy) => $"[yellow]Password Entropy:[/] {entropy:0.##} bits";

    public static string StrengthMessage(string strength) => $"[yellow]Password Strength:[/] {strength}";

    public static readonly Error PasswordLengthError =
        Error.Validation("Password.Length", "Password length must be between 4-64 characters.");

    public static readonly Error PasswordCountError =
        Error.Validation("Password.Count", "Password count must be between 1-100 characters.");

    public static readonly Error PasswordNoCharSetsError =
        Error.Validation(
            "Password.CharacterSetsRequired",
            "The password configuration must include at least one character set for the password to be generated.\r\n" +
            "Please fix your password.config file.");

    public static string DetermineStrength(double entropy) =>
        entropy switch
        {
            < 40 => "Very Weak",
            < 50 => "Weak",
            < 60 => "Good",
            < 120 => "Strong",
            _ => "Very Strong",
        };
}
