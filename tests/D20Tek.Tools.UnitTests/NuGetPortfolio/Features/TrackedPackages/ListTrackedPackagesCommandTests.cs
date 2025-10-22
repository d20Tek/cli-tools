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
        Assert.Contains("Success:", result.Output);
        Assert.Contains("No packages exist", result.Output);
        Assert.Contains("0 packages retrieved.", result.Output);
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
        Assert.Contains("Success:", result.Output);
        Assert.Contains("Test.Package", result.Output);
        Assert.Contains("Default", result.Output);
        Assert.Contains("1 packages retrieved.", result.Output);
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
        Assert.Contains("Success:", result.Output);
        Assert.Contains("Test-Collection-1", result.Output);
        Assert.Contains("Test-Collection-2", result.Output);
        Assert.Contains("Test.Package.1", result.Output);
        Assert.Contains("Test.Package.2", result.Output);
        Assert.Contains("Test.Package.3", result.Output);
        Assert.Contains("Test.Package.4", result.Output);
        Assert.Contains("4 packages retrieved.", result.Output);
    }
}
