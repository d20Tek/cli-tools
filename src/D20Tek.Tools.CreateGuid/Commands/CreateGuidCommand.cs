using D20Tek.Spectre.Console.Extensions.Services;
using D20Tek.Tools.CreateGuid.Services;
using Spectre.Console.Cli;
using System.Text;
using TextCopy;

namespace D20Tek.Tools.CreateGuid.Commands;

internal class CreateGuidCommand : Command<GuidSettings>
{
	private readonly IGuidGenerator _guidGenerator;
	private readonly IGuidFormatter _guidFormatter;
	private readonly IVerbosityWriter _writer;
	private readonly IClipboard _clipboard;

	public CreateGuidCommand(
        IGuidGenerator guidGen,
        IGuidFormatter formatter,
        IVerbosityWriter writer,
        IClipboard clipboard)
	{
		ArgumentNullException.ThrowIfNull(guidGen, nameof(guidGen));
		ArgumentNullException.ThrowIfNull(formatter, nameof(formatter));
		ArgumentNullException.ThrowIfNull(writer, nameof(writer));
		ArgumentNullException.ThrowIfNull(clipboard, nameof(clipboard));

		_guidGenerator = guidGen;
		_guidFormatter = formatter;
		_writer = writer;
		_clipboard = clipboard;
	}

	public override int Execute(CommandContext context, GuidSettings settings)
	{
		_writer.Verbosity = settings.Verbosity;
		_writer.WriteNormal("create-guid: running to generate your GUIDs:");

		var stringBuilder = GenerateGuidStrings(settings);
		CopyToClipboard(stringBuilder, settings);
		SaveOutputFile(stringBuilder, settings);

		_writer.MarkupNormal("[green]Command completed successfully![/]");
		return 0;
	}

	private StringBuilder GenerateGuidStrings(GuidSettings settings)
	{
        var stringBuilder = new StringBuilder();
        foreach (Guid item in _guidGenerator.GenerateGuids(settings.Count, settings.UsesEmptyGuid, settings.UsesUuidV7))
        {
            string text = _guidFormatter.Format(item, settings.Format, settings.UsesUpperCase);
            _writer.WriteSummary(text);
            stringBuilder.AppendLine(text);
        }

		return stringBuilder;
    }

    private void CopyToClipboard(StringBuilder builder, GuidSettings settings)
	{
		if (settings.CopyToClipboard)
		{
			string text = builder.ToString();
			_clipboard.SetText(text);

			_writer.WriteDiagnostics("");
			_writer.WriteDiagnostics("=> Text copied to clipboard:");
			_writer.MarkupDiagnostics("[gray]" + text + "[/]");
		}
	}

	private void SaveOutputFile(StringBuilder builder, GuidSettings settings)
	{
		if (!string.IsNullOrEmpty(settings.OutputFile))
		{
			var directoryName = Path.GetDirectoryName(settings.OutputFile);
			if (directoryName != null)
			{
				Directory.CreateDirectory(directoryName);
			}

			File.WriteAllText(settings.OutputFile, builder.ToString(), Encoding.ASCII);

			_writer.WriteDiagnostics("");
			_writer.WriteDiagnostics("=> Text saved to file (" + settings.OutputFile + ").");
		}
	}
}
