using D20Tek.Functional;
using Spectre.Console;

namespace D20Tek.Tools.DevPomo.Common;

public static class AnsiConsoleExtensions
{
    public static void MarkupLines(this IAnsiConsole console, params string[] messages) =>
        console.MarkupLine(string.Join(Environment.NewLine, messages));

    public static void MarkupLinesConditional(this IAnsiConsole console, bool condition, params string[] prompt) =>
        condition.IfTrueOrElse(() => console.MarkupLines(prompt));

    public static void ContinueOnAnyKey(this IAnsiConsole console, string[] labels) =>
        console.ToIdentity().Iter(c => c.MarkupLines(labels))
                            .Iter(c => c.Input.ReadKey(true));

    public static void DisplayAppHeader(this IAnsiConsole console, string title, Color? color = null) =>
        console.Write(new FigletText(title).Centered().Color(color ?? Color.Green));
}
