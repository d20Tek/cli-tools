//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using Spectre.Console.Cli;

namespace D20Tek.Tools.CreateGuid.Tests.Common
{
    public interface ITestConfigurator : IConfigurator
    {
        IList<CommandMetadata> Commands { get; }

        CommandMetadata? DefaultCommand { get; }

        IList<string[]> Examples { get; }
    }
}
