namespace D20Tek.Tools.DevPassword.Common;

public static class VerbosityWriterExtensions
{
    public const string ErrorLabel = "[red]Error:[/]";

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
