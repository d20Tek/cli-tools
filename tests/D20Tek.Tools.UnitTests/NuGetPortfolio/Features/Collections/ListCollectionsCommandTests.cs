using D20Tek.NuGet.Portfolio;
using D20Tek.NuGet.Portfolio.Domain;
using D20Tek.Tools.UnitTests.NuGetPortfolio.Fakes;

namespace D20Tek.Tools.UnitTests.NuGetPortfolio.Features.Collections;

[TestClass]
public class ListCollectionsCommandTests
{

    [TestMethod]
    public async Task ExecuteAsync_WithNoCollections_ShouldPrintDefaultCollection()
    {
        // arrange
        var context = CommandAppContextFactory.CreateWithMemoryDb();

        // act
        var result = await context.RunAsync(["collection", "list"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(Globals.S_OK, result.ExitCode);
        Assert.Contains("Success:", result.Output);
        Assert.Contains("No collections exist", result.Output);
        Assert.Contains("0 collections retrieved.", result.Output);
    }

    [TestMethod]
    public async Task ExecuteAsync_WithDefaultCollection_ShouldPrintDefaultCollection()
    {
        // arrange
        var db = InMemoryDbContext.InitializeDatabase([CollectionEntity.Create("Default").GetValue()]);
        var context = CommandAppContextFactory.CreateWithMemoryDb(db);

        // act
        var result = await context.RunAsync(["collection", "list"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(Globals.S_OK, result.ExitCode);
        Assert.Contains("Success:", result.Output);
        Assert.Contains("Default", result.Output);
        Assert.Contains("1 collections retrieved.", result.Output);
    }

    [TestMethod]
    public async Task ExecuteAsync_WithMultipleCollections_ShouldPrintsAllCollections()
    {
        // arrange
        var db = InMemoryDbContext.InitializeDatabase(
            [
                CollectionEntity.Create("Test-Collection-1").GetValue(),
                CollectionEntity.Create("Test-Collection-2").GetValue(),
                CollectionEntity.Create("Test-Collection-3").GetValue()
            ]);
        var context = CommandAppContextFactory.CreateWithMemoryDb(db);

        // act
        var result = await context.RunAsync(["collection", "list"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(Globals.S_OK, result.ExitCode);
        Assert.Contains("Success:", result.Output);
        Assert.Contains("Test-Collection-1", result.Output);
        Assert.Contains("Test-Collection-2", result.Output);
        Assert.Contains("Test-Collection-3", result.Output);
        Assert.Contains("3 collections retrieved.", result.Output);
    }
}
