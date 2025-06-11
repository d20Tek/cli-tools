using D20Tek.NuGet.Portfolio.Common.Controls;

namespace D20Tek.NuGet.Portfolio.Common;

internal static class AnsiConsoleExtensions
{
    public static CommandHeader CommandHeader(this IAnsiConsole console) => new(console);

    public static T AskIfDefault<T>(this IAnsiConsole console, T value, string label) =>
        (value.IsDefault() is false) ? value : console.AskWithLabel<T>(label);

    private static T AskWithLabel<T>(this IAnsiConsole console, string label)
    {
        console.WriteLine(label);
        return console.Ask<T>(Globals.AppPrompt);
    }

    private static bool IsDefault<T>(this T value) =>
        value switch
        {
            string s => string.IsNullOrWhiteSpace(s),
            null => true,
            _ => EqualityComparer<T>.Default.Equals(value, default!)
        };
}
