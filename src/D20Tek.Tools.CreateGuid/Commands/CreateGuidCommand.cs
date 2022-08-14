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

        public CreateGuidCommand(IGuidGenerator guidGen)
        {
            ArgumentNullException.ThrowIfNull(guidGen, nameof(guidGen));
            _guidGenerator = guidGen;
        }

        public override int Execute([NotNull] CommandContext context, [NotNull] GuidSettings settings)
        {
            AnsiConsole.WriteLine($"create-guid: running to generate your GUIDs:");

            foreach (var guid in _guidGenerator.GenerateGuids(1, settings.UsesEmptyGuid))
            {
                AnsiConsole.WriteLine(guid.ToString());
            }

            AnsiConsole.MarkupLine($"[green]Command completed successfully![/]");
            return 0;
        }
    }
}