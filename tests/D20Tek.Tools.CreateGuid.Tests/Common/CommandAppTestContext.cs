//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using D20Tek.Spectre.Console.Extensions.Injection;
using Microsoft.Extensions.DependencyInjection;
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
    }
}
