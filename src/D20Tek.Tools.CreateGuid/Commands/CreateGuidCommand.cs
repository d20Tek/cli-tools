using D20Tek.Spectre.Console.Extensions.Services;
using D20Tek.Tools.CreateGuid.Contracts;
using Spectre.Console.Cli;
using TextCopy;

namespace D20Tek.Tools.CreateGuid.Commands;

internal sealed class CreateGuidCommand : Command<GuidSettings>
{
	private readonly CreateGuidState _state;
	private readonly IVerbosityWriter _writer;


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

		_state = new(guidGen, formatter, writer, clipboard);
		_writer = writer;
	}

	public override int Execute(CommandContext context, GuidSettings settings, CancellationToken token)
	{
		_writer.Verbosity = settings.Verbosity;
		_writer.WriteNormal("create-guid: running to generate your GUIDs:");

		_state.GenerateGuidStrings(settings)
			  .CopyToClipboard(settings)
			  .SaveOutputFile(settings);

		_writer.MarkupNormal("[green]Command completed successfully![/]");
		return 0;
	}
}
