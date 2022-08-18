//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using Spectre.Console.Cli;

namespace D20Tek.Tools.CreateGuid.Tests.Common
{
    public class CommandTestContext
    {
        public CommandContext CommandContext { get; }

        public CommandTestContext(
            IRemainingArguments? remaining = null,
            string name = "__defaultTestMethod",
            object? data = null)
        {
            var remainingArgs = remaining ?? new Mock<IRemainingArguments>().Object;
            CommandContext = new CommandContext(remainingArgs, name, data);
        }
    }
}
