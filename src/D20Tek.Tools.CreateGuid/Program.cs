//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using D20Tek.Spectre.Console.Extensions;
using D20Tek.Tools.CreateGuid.Commands;

namespace D20Tek.Tools.CreateGuid
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            return await new CommandAppBuilder()
                             .WithDIContainer()
                             .WithStartup<Startup>()
                             .WithDefaultCommand<CreateGuidCommand>()
                             .Build()
                             .RunAsync(args);
        }
    }
}