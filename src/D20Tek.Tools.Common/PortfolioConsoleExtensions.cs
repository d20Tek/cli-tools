using D20Tek.Tools.Common.Controls;

namespace D20Tek.Tools.Common;

public static class PortfolioConsoleExtensions
{
    private static string GetPromptLabel<T>(string label, T prev) where T : notnull => $"{label} [green]({prev})[/]";

    public static CommandHeader CommandHeader(this IAnsiConsole console) => new(console);

    public static T AskIfDefault<T>(this IAnsiConsole console, T value, string label, string prompt)
        where T : notnull =>
        (value.IsDefault() is false) ? value : console.AskWithLabel<T>(label, prompt);

    private static T AskWithLabel<T>(this IAnsiConsole console, string label, string prompt) where T : notnull =>
        console.ToIdentity()
               .Iter(c => c.WriteLine(label))
               .Map(c => c.Ask<T>(prompt));

    public static T PromptIfDefault<T>(this IAnsiConsole console, T value, string label, string prompt, T previousValue)
        where T : notnull =>
        (value.IsDefault() is false) ? value : console.PromptWithLabel<T>(label, prompt, previousValue);

    private static T PromptWithLabel<T>(this IAnsiConsole console, string label, string prompt, T prev)
        where T : notnull =>
        console.ToIdentity()
               .Iter(c => c.MarkupLine(GetPromptLabel(label, prev)))
               .Map(c => c.Prompt(
                   new TextPrompt<T>(prompt).DefaultValue(prev).HideDefaultValue()));

    private static bool IsDefault<T>(this T value) =>
        value switch
        {
            string s => string.IsNullOrWhiteSpace(s),
            null => true,
            _ => EqualityComparer<T>.Default.Equals(value, default!)
        };

    public static void RenderTableWithTitle(this IAnsiConsole console, string title, Table table) =>
        console.ToIdentity()
               .Iter(c => c.WriteLine())
               .Iter(c => c.CommandHeader().Render(title))
               .Iter(c => c.Write(table));
}
