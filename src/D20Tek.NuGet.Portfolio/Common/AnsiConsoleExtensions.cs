namespace D20Tek.NuGet.Portfolio.Common;

internal static class AnsiConsoleExtensions
{
    public static CommandHeader CommandHeader(this IAnsiConsole console) => new(console);

    public static T AskIfDefault<T>(this IAnsiConsole console, T value, string label) where T : notnull =>
        (value.IsDefault() is false) ? value : console.AskWithLabel<T>(label);

    private static T AskWithLabel<T>(this IAnsiConsole console, string label) where T : notnull =>
        console.ToIdentity()
               .Iter(c => c.WriteLine(label))
               .Map(c => c.Ask<T>(Globals.AppPrompt));

    public static T PromptIfDefault<T>(this IAnsiConsole console, T value, string label, T previousValue)
        where T : notnull =>
        (value.IsDefault() is false) ? value : console.PromptWithLabel<T>(label, previousValue);

    private static T PromptWithLabel<T>(this IAnsiConsole console, string label, T prev) where T : notnull =>
        console.ToIdentity()
               .Iter(c => c.MarkupLine($"{label} [green]({prev})[/]"))
               .Map(c => c.Prompt(
                   new TextPrompt<T>(Globals.AppPrompt).DefaultValue(prev).HideDefaultValue()));

    private static bool IsDefault<T>(this T value) =>
        value switch
        {
            string s => string.IsNullOrWhiteSpace(s),
            null => true,
            _ => EqualityComparer<T>.Default.Equals(value, default!)
        };
}
