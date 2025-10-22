using D20Tek.NuGet.Portfolio;
using D20Tek.Tools.UnitTests.NuGetPortfolio.Fakes;

namespace D20Tek.Tools.UnitTests.NuGetPortfolio.Features.Collections;

[TestClass]
public class AddCollectionCommandTests
{
    [TestMethod]
    public async Task ExecuteAsync_WithValidCollectionName_CreatesCollectionEntity()
    {
        // arrange
        var context = CommandAppContextFactory.CreateWithMemoryDb();

        // act
        var result = await context.RunAsync(["collection", "add", "--name", "Test Collection"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(Globals.S_OK, result.ExitCode);
        Assert.Contains("Success:", result.Output);
        Assert.Contains("'Test Collection'", result.Output);
    }

    [TestMethod]
    public async Task ExecuteAsync_WithEmptyCollectionName_CreatesCollectionEntity()
    {
        // arrange
        var context = CommandAppContextFactory.CreateWithMemoryDb();
        context.Console.TestInput.PushTextWithEnter("Interactive collection");

        // act
        var result = await context.RunAsync(["collection", "add"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(Globals.S_OK, result.ExitCode);
        Assert.Contains("Success:", result.Output);
        Assert.Contains("'Interactive collection'", result.Output);
    }

    [TestMethod]
    public async Task ExecuteAsync_WithLongCollectionName_ReturnsValidationError()
    {
        // arrange
        var context = CommandAppContextFactory.CreateWithMemoryDb();
        context.Console.TestInput.PushTextWithEnter(
            "Super long text that goes over max length: a;sdlkjf alkjfad ;ajf;lka sdfaj;lkjfa;lkdjf a;jf a;lkdsjf a;lkj fal;kdjf a;lkjf a;");

        // act
        var result = await context.RunAsync(["collection", "add"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(Globals.E_FAIL, result.ExitCode);
        Assert.Contains("Error:", result.Output);
        Assert.Contains("64 characters or less", result.Output);
    }
}
