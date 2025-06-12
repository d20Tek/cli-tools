using D20Tek.NuGet.Portfolio;
using D20Tek.NuGet.Portfolio.Domain;
using D20Tek.NuGet.Portfolio.Persistence;
using D20Tek.Tools.UnitTests.NuGetPortfolio.Fakes;

namespace D20Tek.Tools.UnitTests.NuGetPortfolio.Features.Collections;

[TestClass]
public class EditCollectionCommandTests
{

    [TestMethod]
    public async Task ExecuteAsync_WithValidProperties_UpdatesCollectionEntity()
    {
        // arrange
        var context = CommandAppContextFactory.CreateWithMemoryDb(CreateDatabaseWithCollections());

        // act
        var result = await context.RunAsync(["collection", "edit", "--id", "2", "--name", "Test-Collection-Updated"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(Globals.S_OK, result.ExitCode);
        StringAssert.Contains(result.Output, "Success:");
        StringAssert.Contains(result.Output, "'Test-Collection-Updated'");
    }

    [TestMethod]
    public async Task ExecuteAsync_WithMissingId_ReturnsNotFoundError()
    {
        // arrange
        var context = CommandAppContextFactory.CreateWithMemoryDb();

        // act
        var result = await context.RunAsync(["collection", "edit", "--id", "999", "--name", "Test-Collection-Updated"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(Globals.E_FAIL, result.ExitCode);
        StringAssert.Contains(result.Output, "Error:");
        StringAssert.Contains(result.Output, "not found");
    }

    [TestMethod]
    public async Task ExecuteAsync_WithEmptyProperties_CreatesCollectionEntity()
    {
        // arrange
        var context = CommandAppContextFactory.CreateWithMemoryDb(CreateDatabaseWithCollections());
        context.Console.TestInput.PushTextWithEnter("Interactive collection");
        context.Console.TestInput.PushTextWithEnter("2");

        // act
        var result = await context.RunAsync(["collection", "edit"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(Globals.S_OK, result.ExitCode);
        StringAssert.Contains(result.Output, "Success:");
        StringAssert.Contains(result.Output, "'Interactive collection'");
    }

    [TestMethod]
    public async Task ExecuteAsync_WithLongCollectionName_ReturnsValidationError()
    {
        // arrange
        var context = CommandAppContextFactory.CreateWithMemoryDb(CreateDatabaseWithCollections());
        context.Console.TestInput.PushTextWithEnter(
            "Super long text that goes over max length: a;sdlkjf alkjfad ;ajf;lka sdfaj;lkjfa;lkdjf a;jf a;lkdsjf a;lkj fal;kdjf a;lkjf a;");

        // act
        var result = await context.RunAsync(["collection", "edit", "--id", "2"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(Globals.E_FAIL, result.ExitCode);
        StringAssert.Contains(result.Output, "Error:");
        StringAssert.Contains(result.Output, "64 characters or less");
    }

    private AppDbContext CreateDatabaseWithCollections() =>
        InMemoryDbContext.InitializeDatabase(
            [
                CollectionEntity.Create("Test-Collection-1").GetValue(),
                CollectionEntity.Create("Test-Collection-2").GetValue(),
                CollectionEntity.Create("Test-Collection-3").GetValue()
            ]);

}
