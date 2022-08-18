//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using D20Tek.Spectre.Console.Extensions.Injection;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using Spectre.Console.Cli;

namespace D20Tek.Tools.CreateGuid.Tests.Common
{
    public class CommandAppTestContext
    {
        public ITypeRegistrar Registrar { get; }

        public ITypeResolver Resolver => Registrar.Build();

        public ITestConfigurator Configurator { get; }

        public CommandAppTestContext()
        {
            Registrar = new DependencyInjectionTypeRegistrar(new ServiceCollection());
            Configurator = new FakeConfigurator(Registrar);
        }

        public async Task<CommandAppBasicResult> RunAsync(
            Func<string[], Task<int>> mainEntryPointAsync,
            string commandLine)
        {
            var args = commandLine.Split(' ', '\t');
            return await RunAsync(mainEntryPointAsync, args);
        }

        public async Task<CommandAppBasicResult> RunAsync(
            Func<string[], Task<int>> mainEntryPointAsync,
            string[] args)
        {
            ArgumentNullException.ThrowIfNull(mainEntryPointAsync, nameof(mainEntryPointAsync));

            AnsiConsole.Record();
            var exitCode = await mainEntryPointAsync(args);

            return new CommandAppBasicResult(exitCode, AnsiConsole.ExportText());
        }

        public CommandAppBasicResult Run(Func<string[], int> mainEntryPoint, string commandLine)
        {
            var args = commandLine.Split(' ', '\t');
            return Run(mainEntryPoint, args);
        }

        public CommandAppBasicResult Run(Func<string[], int> mainEntryPoint, string[] args)
        {
            ArgumentNullException.ThrowIfNull(mainEntryPoint, nameof(mainEntryPoint));

            AnsiConsole.Record();
            var exitCode = mainEntryPoint(args);

            return new CommandAppBasicResult(exitCode, AnsiConsole.ExportText());
        }
    }
}
