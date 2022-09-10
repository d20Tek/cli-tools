//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using D20Tek.Spectre.Console.Extensions.Services;
using D20Tek.Tools.CreateGuid.Services;
using Spectre.Console.Cli;
using System.Diagnostics.CodeAnalysis;

namespace D20Tek.Tools.CreateGuid.Commands
{
    internal class CreateGuidCommand : Command<GuidSettings>
    {
        private readonly IGuidGenerator _guidGenerator;
        private readonly IGuidFormatter _guidFormatter;
        private readonly IVerbosityWriter _writer;

        public CreateGuidCommand(IGuidGenerator guidGen, IGuidFormatter formatter, IVerbosityWriter writer)
        {
            ArgumentNullException.ThrowIfNull(guidGen, nameof(guidGen));
            ArgumentNullException.ThrowIfNull(formatter, nameof(formatter));
            ArgumentNullException.ThrowIfNull(writer, nameof(writer));

            _guidGenerator = guidGen;
            _guidFormatter = formatter;
            _writer = writer;
        }

        public override int Execute([NotNull] CommandContext context, [NotNull] GuidSettings settings)
        {
            _writer.Verbosity = settings.Verbosity;
            _writer.WriteNormal($"create-guid: running to generate your GUIDs:");

            foreach (var guid in _guidGenerator.GenerateGuids(settings.Count, settings.UsesEmptyGuid))
            {
                _writer.WriteSummary(
                    _guidFormatter.Format(guid, settings.Format, settings.UsesUpperCase));
            }

            _writer.MarkupNormal($"[green]Command completed successfully![/]");
            return 0;
        }
    }
}