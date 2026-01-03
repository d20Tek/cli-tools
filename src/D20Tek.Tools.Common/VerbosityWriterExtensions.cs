namespace D20Tek.Tools.Common;

public static class VerbosityWriterExtensions
{
    public static IVerbosityWriter RenderCommandTitle(this IVerbosityWriter writer, string title, VerbosityLevel verbosityLevel)
    {
        writer.Verbosity = verbosityLevel;
        writer.WriteNormal(title);
        writer.WriteNormal();

        return writer;
    }
}
