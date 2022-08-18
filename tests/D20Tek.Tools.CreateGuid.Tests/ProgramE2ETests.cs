//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using D20Tek.Tools.CreateGuid.Tests.Common;

namespace D20Tek.Tools.CreateGuid.Tests
{
    [TestClass]
    public class ProgramE2ETests
    {
        [TestMethod]
        public async Task CreateGuidOperation_Default()
        {
            // arrange

            // act
            var result = await CommandAppE2ERunner.RunAsync(Program.Main, Array.Empty<string>());

            // assert
            Assert.AreEqual(0, result.ExitCode);
            StringAssert.Contains(result.Output, "create-guid");
            StringAssert.Contains(result.Output, "completed successfully");
        }

        [TestMethod]
        public async Task CreateGuidOperation_EmptyGuid()
        {
            // arrange
            var args = "-f Default -e -u";

            // act
            var result = await CommandAppE2ERunner.RunAsync(Program.Main, args);

            // assert
            Assert.AreEqual(0, result.ExitCode);
            StringAssert.Contains(result.Output, "create-guid");
            StringAssert.Contains(result.Output, "00000000-0000-0000-0000-000000000000");
            StringAssert.Contains(result.Output, "completed successfully");
        }


        [TestMethod]
        public async Task CreateGuidOperation_MultipleGuids()
        {
            // arrange
            var args = "--count 5";

            // act
            var result = await CommandAppE2ERunner.RunAsync(Program.Main, args);

            // assert
            Assert.AreEqual(0, result.ExitCode);
            StringAssert.Contains(result.Output, "create-guid");
            StringAssert.Contains(result.Output, "completed successfully");
        }
    }
}