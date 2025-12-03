global using D20Tek.Spectre.Console.Extensions;
global using D20Tek.Spectre.Console.Extensions.Injection;
global using D20Tek.Spectre.Console.Extensions.Services;
global using D20Tek.Spectre.Console.Extensions.Settings;
global using Spectre.Console.Cli;
global using System.ComponentModel;
global using System.Text;

using D20Tek.Tools.CreateGuid.Commands;
using D20Tek.Tools.CreateGuid.Contracts;
using D20Tek.Tools.CreateGuid.Services;
using TextCopy;

namespace D20Tek.Tools.CreateGuid;

internal sealed class Startup : StartupBase
{
	public override void ConfigureServices(ITypeRegistrar registrar)
	{
		registrar.WithConsoleVerbosityWriter();
		registrar.WithLifetimes().RegisterSingleton<IGuidGenerator, GuidGenerator>()
								 .RegisterSingleton<IGuidFormatter, GuidFormatter>()
								 .RegisterSingleton<IClipboard, Clipboard>();
	}

	public override IConfigurator ConfigureCommands(IConfigurator config)
	{
		config.CaseSensitivity(CaseSensitivity.None);
        config.SetApplicationName("create-guid");
        config.ValidateExamples();

		config.AddCommand<CreateGuidCommand>("generate")
			  .WithAlias("gen")
			  .WithDescription("Default command that generates GUIDs in the appropriate format.")
			  .WithExample(["gen", "-c", "1", "-f", "default", "-v", "normal"]);

		return config;
	}
}
