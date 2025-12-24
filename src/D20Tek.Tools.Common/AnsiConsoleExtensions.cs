namespace D20Tek.Tools.Common;

public static class AnsiConsoleExtensions
{
    public static void Beep(this IAnsiConsole _) => Console.Beep();

    public static void MarkupLines(this IAnsiConsole console, params string[] messages) =>
        console.MarkupLine(string.Join(Environment.NewLine, messages));

    public static void MarkupLinesConditional(this IAnsiConsole console, bool condition, params string[] prompt) =>
        condition.IfTrueOrElse(() => console.MarkupLines(prompt));

    public static void ContinueOnAnyKey(this IAnsiConsole console, string[] labels) =>
        console.ToIdentity().Iter(c => c.MarkupLines(labels))
                            .Iter(c => c.Input.ReadKey(true));

    public static void DisplayAppHeader(
        this IAnsiConsole console,
        string title,
        bool showHeader,
        Justify? justification = Justify.Center,
        Color? color = null)
    {
        if (showHeader) console.Write(new FigletText(title).Justify(justification).Color(color ?? Color.Green));
    }
}
