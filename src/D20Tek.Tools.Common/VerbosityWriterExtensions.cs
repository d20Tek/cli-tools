namespace D20Tek.Tools.Common;

public static class VerbosityWriterExtensions
{
    public const string ErrorLabel = "[red]Error:[/]";

    public static IVerbosityWriter RenderCommandTitle(this IVerbosityWriter writer, string title, VerbosityLevel verbosityLevel)
    {
        writer.Verbosity = verbosityLevel;
        writer.WriteNormal(title);
        writer.WriteNormal();

        return writer;
    }

    public static int RenderCompletion(this IVerbosityWriter writer, string completionMessage)
    {
        ArgumentNullException.ThrowIfNull(writer);
        writer.MarkupNormal(completionMessage);
        return 0;
    }

    public static int RenderErrors(this IVerbosityWriter writer, Error[] errors)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(errors);

        errors.ForEach(e => writer.MarkupSummary($"{ErrorLabel} {e.Message}"));
        return -1;
    }
}
