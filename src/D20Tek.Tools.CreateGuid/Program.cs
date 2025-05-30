using D20Tek.Spectre.Console.Extensions;
using D20Tek.Tools.CreateGuid.Commands;
using D20Tek.Tools.CreateGuid;

return await new CommandAppBuilder().WithDIContainer()
                                    .WithStartup<Startup>()
                                    .WithDefaultCommand<CreateGuidCommand>()
                                    .Build()
                                    .RunAsync(args);
