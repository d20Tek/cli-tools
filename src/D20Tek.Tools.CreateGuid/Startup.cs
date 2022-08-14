//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using D20Tek.Spectre.Console.Extensions;
using D20Tek.Tools.CreateGuid.Commands;
using D20Tek.Tools.CreateGuid.Services;
using Spectre.Console.Cli;

namespace D20Tek.Tools.CreateGuid
{
    internal class Startup : StartupBase
    {
        public override void ConfigureServices(ITypeRegistrar registrar)
        {
            // register services here...
            registrar.Register(typeof(IGuidGenerator), typeof(GuidGenerator));
        }

        public override IConfigurator ConfigureCommands(IConfigurator config)
        {
            config.CaseSensitivity(CaseSensitivity.None);
            config.SetApplicationName("create-guid");
            config.ValidateExamples();

            config.AddCommand<CreateGuidCommand>("generate")
                .WithAlias("gen")
                .WithDescription("Default command that generates GUIDs in the appropriate format.")
                .WithExample(new[] { "gen" });

            return config;
        }
    }
}