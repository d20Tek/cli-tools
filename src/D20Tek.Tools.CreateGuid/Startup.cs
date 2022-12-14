//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using D20Tek.Spectre.Console.Extensions;
using D20Tek.Spectre.Console.Extensions.Services;
using D20Tek.Tools.CreateGuid.Commands;
using D20Tek.Tools.CreateGuid.Services;
using Spectre.Console.Cli;
using TextCopy;

namespace D20Tek.Tools.CreateGuid
{
    internal class Startup : StartupBase
    {
        public override void ConfigureServices(ITypeRegistrar registrar)
        {
            // register services here...
            registrar.WithConsoleVerbosityWriter();
            registrar.Register(typeof(IGuidGenerator), typeof(GuidGenerator));
            registrar.Register(typeof(IGuidFormatter), typeof(GuidFormatter));
            registrar.Register(typeof(IClipboard), typeof(Clipboard));
        }

        public override IConfigurator ConfigureCommands(IConfigurator config)
        {
            config.CaseSensitivity(CaseSensitivity.None);
            config.SetApplicationName("create-guid");
            config.ValidateExamples();

            config.AddCommand<CreateGuidCommand>("generate")
                .WithAlias("gen")
                .WithDescription("Default command that generates GUIDs in the appropriate format.")
                .WithExample(new[] { "gen", "-c", "1", "-f", "default", "-v", "normal" });

            return config;
        }
    }
}