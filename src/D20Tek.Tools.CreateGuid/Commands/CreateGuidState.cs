using D20Tek.Spectre.Console.Extensions.Services;
using D20Tek.Tools.CreateGuid.Contracts;
using System.Text;
using TextCopy;

namespace D20Tek.Tools.CreateGuid.Commands;

internal sealed class CreateGuidState
{
    private readonly IGuidGenerator _guidGenerator;
    private readonly IGuidFormatter _guidFormatter;
    private readonly IVerbosityWriter _writer;
    private readonly IClipboard _clipboard;
    private readonly StringBuilder _stringBuilder;

    public CreateGuidState(IGuidGenerator guidGen, IGuidFormatter formatter, IVerbosityWriter writer, IClipboard clipboard)
    {
        _guidGenerator = guidGen;
        _guidFormatter = formatter;
        _writer = writer;
        _clipboard = clipboard;
        _stringBuilder = new StringBuilder();
    }

    internal CreateGuidState CopyToClipboard(GuidSettings settings)
    {
        if (settings.CopyToClipboard)
        {
            string text = _stringBuilder.ToString();
            _clipboard.SetText(text);

            _writer.WriteDiagnostics();
            _writer.WriteDiagnostics("=> Text copied to clipboard:");
            _writer.MarkupDiagnostics($"[gray]{text}[/]");
        }

        return this;
    }

    internal CreateGuidState GenerateGuidStrings(GuidSettings settings)
    {
        foreach (Guid item in _guidGenerator.GenerateGuids(settings.Count, settings.UsesEmptyGuid, settings.UsesUuidV7))
        {
            string text = _guidFormatter.Format(item, settings.Format, settings.UsesUpperCase);
            _writer.WriteSummary(text);
            _stringBuilder.AppendLine(text);
        }

        return this;
    }

    internal CreateGuidState SaveOutputFile(GuidSettings settings)
    {
        if (!string.IsNullOrEmpty(settings.OutputFile))
        {
            FileSaver.WriteText(settings.OutputFile, _stringBuilder.ToString());
            _writer.WriteDiagnostics();
            _writer.WriteDiagnostics($"=> Text saved to file ({settings.OutputFile}).");
        }

        return this;
    }
}
