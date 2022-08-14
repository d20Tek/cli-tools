//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics.CodeAnalysis;

namespace D20Tek.Tools.CreateGuid.Commands
{
    internal class CreateGuidCommand : Command<GuidSettings>
    {
        public CreateGuidCommand()
        {
        }

        public override int Execute([NotNull] CommandContext context, [NotNull] GuidSettings settings)
        {
            AnsiConsole.MarkupLine($"CommandApp running {this.GetType().Assembly.FullName}");
            AnsiConsole.MarkupLine($"=> Executing command - {context.Name}.");

            AnsiConsole.MarkupLine($"[green]Command completed successfully![/]");

            return 0;
        }
    }
}