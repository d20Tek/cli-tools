using D20Tek.NuGet.Portfolio;
using D20Tek.NuGet.Portfolio.Abstractions;
using D20Tek.NuGet.Portfolio.Domain;
using D20Tek.Tools.UnitTests.NuGetPortfolio.Fakes;

namespace D20Tek.Tools.UnitTests.NuGetPortfolio.Features.PackageDownloads;

[TestClass]
public class GetDownloadsByPackageIdCommandTests
{
    [TestMethod]
    public async Task ExecuteAsync_WithPackageId_ShouldGetDownloadCount()
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

        context.Registrar.RegisterInstance(typeof(INuGetSearchClient), new FakeNuGetSearchClient(42));

        // act
        var result = await context.RunAsync(["get-current", "for-id", "--id", "3"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(Globals.S_OK, result.ExitCode);
        StringAssert.Contains(result.Output, "Success:");
        StringAssert.Contains(result.Output, "Test.Package.3");
        StringAssert.Contains(result.Output, "42 downloads");
    }

    [TestMethod]
    public async Task ExecuteAsync_WithMissingTrackedPackageId_ShouldRemoveEntity()
    {
        // arrange
        var context = CommandAppContextFactory.CreateWithMemoryDb();
        context.Registrar.RegisterInstance(typeof(INuGetSearchClient), new ErrorNuGetSearchClient());

        // act
        var result = await context.RunAsync(["get-current", "for-id", "--id", "3"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(Globals.E_FAIL, result.ExitCode);
        Assert.Contains("Error:", result.Output);
        Assert.Contains("not found", result.Output);
    }

    [TestMethod]
    public async Task ExecuteAsync_WithInvalidPackageId_ShouldGetDownloadCount()
    {
        // arrange
        var db = InMemoryDbContext.InitializeDatabase(
            [
                CollectionEntity.Create("Test-Collection-1").GetValue(),
            ],
            [
                TrackedPackageEntity.Create("Test.Package.1", 1).GetValue(),
            ]);
        var context = CommandAppContextFactory.CreateWithMemoryDb(db);

        context.Registrar.RegisterInstance(typeof(INuGetSearchClient), new ErrorNuGetSearchClient());

        // act
        var result = await context.RunAsync(["get-current", "for-id", "--id", "1"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(Globals.E_FAIL, result.ExitCode);
        Assert.Contains("Error:", result.Output);
        Assert.Contains("Test.Package.1", result.Output);
        Assert.Contains("not found", result.Output);
    }
}
