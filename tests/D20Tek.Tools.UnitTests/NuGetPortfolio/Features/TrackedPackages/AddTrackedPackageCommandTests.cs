using D20Tek.NuGet.Portfolio;
using D20Tek.NuGet.Portfolio.Domain;
using D20Tek.NuGet.Portfolio.Persistence;
using D20Tek.Tools.UnitTests.NuGetPortfolio.Fakes;

namespace D20Tek.Tools.UnitTests.NuGetPortfolio.Features.TrackedPackages;

[TestClass]
public class AddTrackedPackageCommandTests
{
    [TestMethod]
    public async Task ExecuteAsync_WithValidPackageProperties_CreatesTrackedPackageEntity()
    {
        // arrange
        var context = CommandAppContextFactory.CreateWithMemoryDb(CreateDatabaseWithCollections());

        // act
        var result = await context.RunAsync(["package", "add", "--package-id", "test-package-1", "--collection-id", "1"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(Globals.S_OK, result.ExitCode);
        Assert.Contains("Success:", result.Output);
        Assert.Contains("'test-package-1'", result.Output);
    }

    [TestMethod]
    public async Task ExecuteAsync_WithEmptyProperties_CreatesTrackedPackageEntity()
    {
        // arrange
        var context = CommandAppContextFactory.CreateWithMemoryDb(CreateDatabaseWithCollections());
        context.Console.TestInput.PushTextWithEnter("interactive-package-id");
        context.Console.TestInput.PushTextWithEnter("1");

        // act
        var result = await context.RunAsync(["package", "add"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(Globals.S_OK, result.ExitCode);
        Assert.Contains("Success:", result.Output);
        Assert.Contains("'interactive-package-id'", result.Output);
    }

    [TestMethod]
    public async Task ExecuteAsync_WithLongCollectionName_ReturnsValidationError()
    {
        // arrange
        var context = CommandAppContextFactory.CreateWithMemoryDb(CreateDatabaseWithCollections());
        context.Console.TestInput.PushTextWithEnter(
            "Super long text that goes over max length: a;sdlkjf alkjfad ;ajf;lka sdfaj;lkjfa;lkdjf a;jf a;lkdsjf a;lkj fal;kdjf a;lkjf a;a;sdlkjf alkjfad ;ajf;lka sdfaj;lkjfa;lkdjf a;jf a;lkdsjf a;lkj fal;kdjf a;lkjf a;a;sdlkjf alkjfad ;ajf;lka sdfaj;lkjfa;lkdjf a;jf a;lkdsjf a;lkj fal;kdjf a;lkjf a;");

        // act
        var result = await context.RunAsync(["package", "add", "--collection-id", "1"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(Globals.E_FAIL, result.ExitCode);
        Assert.Contains("Error:", result.Output);
        Assert.Contains("256 characters or less", result.Output);
    }

    [TestMethod]
    public async Task ExecuteAsync_WithMissingCollectionId_ReturnsNotFoundError()
    {
        // arrange
        var context = CommandAppContextFactory.CreateWithMemoryDb();

        // act
        var result = await context.RunAsync(["package", "add", "--package-id", "test-package-2", "--collection-id", "404"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(Globals.E_FAIL, result.ExitCode);
        Assert.Contains("Error:", result.Output);
        Assert.Contains("not found", result.Output);
    }

    private AppDbContext CreateDatabaseWithCollections() =>
        InMemoryDbContext.InitializeDatabase(
        [
            CollectionEntity.Create("Default").GetValue(),
        ]);
}
