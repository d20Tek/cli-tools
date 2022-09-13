//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using Spectre.Console.Cli;

namespace D20Tek.Tools.CreateGuid.Tests.Mocks
{
    internal static class MockCommandContext
    {
        internal static CommandContext Get(string name = "_defaultName")
        {
            var remaining = new Mock<IRemainingArguments>().Object;
            return new CommandContext(remaining, name, null);
        }
    }
}
