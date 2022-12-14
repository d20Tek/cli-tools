//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using D20Tek.Spectre.Console.Extensions.Services;
using D20Tek.Tools.CreateGuid.Services;
using Spectre.Console.Cli;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using TextCopy;

namespace D20Tek.Tools.CreateGuid.Commands
{
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

        public override int Execute([NotNull] CommandContext context, [NotNull] GuidSettings settings)
        {
            _writer.Verbosity = settings.Verbosity;
            _writer.WriteNormal($"create-guid: running to generate your GUIDs:");

            var textBuffer = new StringBuilder();
            foreach (var guid in _guidGenerator.GenerateGuids(settings.Count, settings.UsesEmptyGuid))
            {
                var guidText = _guidFormatter.Format(guid, settings.Format, settings.UsesUpperCase);
                _writer.WriteSummary(guidText);
                textBuffer.AppendLine(guidText);
            }

            CopyToClipboard(textBuffer, settings);
            SaveOutputFile(textBuffer, settings);
            _writer.MarkupNormal($"[green]Command completed successfully![/]");
            return 0;
        }

        private void CopyToClipboard(StringBuilder builder, GuidSettings settings)
        {
            if (settings.CopyToClipboard == true)
            {
                var text = builder.ToString();
                _clipboard.SetText(text);

                _writer.WriteDiagnostics();
                _writer.WriteDiagnostics("=> Text copied to clipboard:");
                _writer.MarkupDiagnostics($"[gray]{text}[/]");
            }
        }

        private void SaveOutputFile(StringBuilder builder, GuidSettings settings)
        {
            if (string.IsNullOrEmpty(settings.OutputFile) == false)
            {
                // ensure directory exists.
                var directoryPath = Path.GetDirectoryName(settings.OutputFile);
                if (directoryPath != null)
                {
                    Directory.CreateDirectory(directoryPath);
                }

                // save output file.
                File.WriteAllText(settings.OutputFile, builder.ToString(), Encoding.ASCII);

                // write diagnostic info.
                _writer.WriteDiagnostics();
                _writer.WriteDiagnostics($"=> Text saved to file ({settings.OutputFile}).");
            }
        }
    }
}