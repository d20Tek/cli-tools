using D20Tek.NuGet.Portfolio;

namespace D20Tek.Tools.UnitTests.NuGetPortfolio.Features.Collections;

[TestClass]
public class CollectionE2ETests
{
    [TestMethod]
    public async Task Run_WithDefaultArgs()
    {
        // arrange

        // act
        var result = await CommandAppE2ERunner.RunAsync(
            Program.Main, ["collection", "add", "--name", "E2E Test Collection"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(Globals.S_OK, result.ExitCode);
        Assert.Contains("Success:", result.Output);
        Assert.Contains("'E2E Test Collection'", result.Output);
    }
}
