using D20Tek.Functional;
using D20Tek.NuGet.Portfolio;
using D20Tek.NuGet.Portfolio.Domain;
using D20Tek.NuGet.Portfolio.Persistence;
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
        StringAssert.Contains(result.Output, "Success:");
        StringAssert.Contains(result.Output, "No collections exist");
        StringAssert.Contains(result.Output, "0 collections retrieved.");
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
        StringAssert.Contains(result.Output, "Success:");
        StringAssert.Contains(result.Output, "Default");
        StringAssert.Contains(result.Output, "1 collections retrieved.");
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
        StringAssert.Contains(result.Output, "Success:");
        StringAssert.Contains(result.Output, "Test-Collection-1");
        StringAssert.Contains(result.Output, "Test-Collection-2");
        StringAssert.Contains(result.Output, "Test-Collection-3");
        StringAssert.Contains(result.Output, "3 collections retrieved.");
    }
}
