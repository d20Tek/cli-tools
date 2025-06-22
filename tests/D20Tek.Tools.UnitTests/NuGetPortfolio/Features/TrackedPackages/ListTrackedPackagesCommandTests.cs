using D20Tek.NuGet.Portfolio;
using D20Tek.NuGet.Portfolio.Domain;
using D20Tek.Tools.UnitTests.NuGetPortfolio.Fakes;

namespace D20Tek.Tools.UnitTests.NuGetPortfolio.Features.TrackedPackages;

[TestClass]
public class ListTrackedPackagesCommandTests
{
    [TestMethod]
    public async Task ExecuteAsync_WithNoPackages_ShouldPrintDefaultCollection()
    {
        // arrange
        var context = CommandAppContextFactory.CreateWithMemoryDb();

        // act
        var result = await context.RunAsync(["package", "list"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(Globals.S_OK, result.ExitCode);
        StringAssert.Contains(result.Output, "Success:");
        StringAssert.Contains(result.Output, "No packages exist");
        StringAssert.Contains(result.Output, "0 packages retrieved.");
    }

    [TestMethod]
    public async Task ExecuteAsync_WithDefaultCollectionAndSinglePackage_ShouldPrintPackage()
    {
        // arrange
        var db = InMemoryDbContext.InitializeDatabase(
            [CollectionEntity.Create("Default").GetValue()],
            [TrackedPackageEntity.Create("Test.Package", 1).GetValue()]);
        var context = CommandAppContextFactory.CreateWithMemoryDb(db);

        // act
        var result = await context.RunAsync(["package", "list"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(Globals.S_OK, result.ExitCode);
        StringAssert.Contains(result.Output, "Success:");
        StringAssert.Contains(result.Output, "Test.Package");
        StringAssert.Contains(result.Output, "Default");
        StringAssert.Contains(result.Output, "1 packages retrieved.");
    }

    [TestMethod]
    public async Task ExecuteAsync_WithMultiplePackages_ShouldPrintsAllPackages()
    {
        // arrange
        var db = InMemoryDbContext.InitializeDatabase(
            [
                CollectionEntity.Create("Test-Collection-1").GetValue(),
                CollectionEntity.Create("Test-Collection-2").GetValue(),
            ],
            [
                TrackedPackageEntity.Create("Test.Package.1", 1).GetValue(),
                TrackedPackageEntity.Create("Test.Package.2", 1).GetValue(),
                TrackedPackageEntity.Create("Test.Package.3", 1).GetValue(),
                TrackedPackageEntity.Create("Test.Package.4", 2).GetValue(),
            ]);
        var context = CommandAppContextFactory.CreateWithMemoryDb(db);

        // act
        var result = await context.RunAsync(["package", "list"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(Globals.S_OK, result.ExitCode);
        StringAssert.Contains(result.Output, "Success:");
        StringAssert.Contains(result.Output, "Test-Collection-1");
        StringAssert.Contains(result.Output, "Test-Collection-2");
        StringAssert.Contains(result.Output, "Test.Package.1");
        StringAssert.Contains(result.Output, "Test.Package.2");
        StringAssert.Contains(result.Output, "Test.Package.3");
        StringAssert.Contains(result.Output, "Test.Package.4");
        StringAssert.Contains(result.Output, "4 packages retrieved.");
    }
}
