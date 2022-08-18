//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using D20Tek.Tools.CreateGuid.Services;
using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics.CodeAnalysis;

namespace D20Tek.Tools.CreateGuid.Commands
{
    internal class CreateGuidCommand : Command<GuidSettings>
    {
        private readonly IGuidGenerator _guidGenerator;
        private readonly IGuidFormatter _guidFormatter;

        public CreateGuidCommand(IGuidGenerator guidGen, IGuidFormatter formatter)
        {
            ArgumentNullException.ThrowIfNull(guidGen, nameof(guidGen));
            ArgumentNullException.ThrowIfNull(formatter, nameof(formatter));

            _guidGenerator = guidGen;
            _guidFormatter = formatter;
        }

        public override int Execute([NotNull] CommandContext context, [NotNull] GuidSettings settings)
        {
            AnsiConsole.WriteLine($"create-guid: running to generate your GUIDs:");

            foreach (var guid in _guidGenerator.GenerateGuids(settings.Count, settings.UsesEmptyGuid))
            {
                AnsiConsole.WriteLine(
                    _guidFormatter.Format(guid, settings.Format, settings.UsesUpperCase));
            }

            AnsiConsole.MarkupLine($"[green]Command completed successfully![/]");
            return 0;
        }
    }
}