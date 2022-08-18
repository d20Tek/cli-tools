//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using Spectre.Console;

namespace D20Tek.Tools.CreateGuid.Tests.Common
{
    public static class CommandAppE2ERunner
    {
        public static async Task<CommandAppBasicResult> RunAsync(
            Func<string[], Task<int>> mainEntryPointAsync,
            string commandLine)
        {
            var args = commandLine.Split(' ', '\t');
            return await RunAsync(mainEntryPointAsync, args);
        }

        public static async Task<CommandAppBasicResult> RunAsync(
            Func<string[], Task<int>> mainEntryPointAsync,
            string[] args)
        {
            ArgumentNullException.ThrowIfNull(mainEntryPointAsync, nameof(mainEntryPointAsync));

            AnsiConsole.Record();
            var exitCode = await mainEntryPointAsync(args);

            return new CommandAppBasicResult(exitCode, AnsiConsole.ExportText());
        }

        public static CommandAppBasicResult Run(Func<string[], int> mainEntryPoint, string commandLine)
        {
            var args = commandLine.Split(' ', '\t');
            return Run(mainEntryPoint, args);
        }

        public static CommandAppBasicResult Run(Func<string[], int> mainEntryPoint, string[] args)
        {
            ArgumentNullException.ThrowIfNull(mainEntryPoint, nameof(mainEntryPoint));

            AnsiConsole.Record();
            var exitCode = mainEntryPoint(args);

            return new CommandAppBasicResult(exitCode, AnsiConsole.ExportText());
        }
    }
}
