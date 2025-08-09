using D20Tek.Spectre.Console.Extensions;
using D20Tek.Tools.DevPomo;
using D20Tek.Tools.DevPomo.Commands;

return await new CommandAppBuilder().WithDIContainer()
                                    .WithStartup<Startup>()
                                    .WithDefaultCommand<RunTimerCommand>()
                                    .Build()
                                    .RunAsync(args);