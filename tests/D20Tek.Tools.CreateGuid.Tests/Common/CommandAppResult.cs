//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------

namespace D20Tek.Tools.CreateGuid.Tests.Common
{
    public class CommandAppBasicResult
    {
        public int ExitCode { get; }

        public string Output { get; }

        public CommandAppBasicResult(int exitCode, string output)
        {
            ExitCode = exitCode;
            Output = output ?? string.Empty;
        }
    }
}
