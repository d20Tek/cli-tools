using D20Tek.NuGet.Portfolio;
using D20Tek.NuGet.Portfolio.Common;
using D20Tek.NuGet.Portfolio.Domain;
using D20Tek.Tools.UnitTests.NuGetPortfolio.Fakes;

namespace D20Tek.Tools.UnitTests.NuGetPortfolio.Features.PackageDownloads;

[TestClass]
public class GetDownloadsByCollectionIdCommandTests
{
    [TestMethod]
    public async Task ExecuteAsync_WithCollectionTrackedPackages_ShouldRenderTable()
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
                TrackedPackageEntity.Create("Test.Package.3", 2).GetValue(),
            ]);
        var context = CommandAppContextFactory.CreateWithMemoryDb(db);

        context.Registrar.RegisterInstance(typeof(INuGetSearchClient), new FakeNuGetSearchClient(42));

        // act
        var result = await context.RunAsync(["get-current", "for-coll", "--collection-id", "1"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(Globals.S_OK, result.ExitCode);
        StringAssert.Contains(result.Output, "Success:");
        StringAssert.Contains(result.Output, "Test.Package.1");
        StringAssert.Contains(result.Output, "Test.Package.2");
        Assert.IsFalse(result.Output.Contains("Test.Package.3"));
        StringAssert.Contains(result.Output, "42");
        StringAssert.Contains(result.Output, "84");
        StringAssert.Contains(result.Output, "for 2 tracked packages");
    }

    [TestMethod]
    public async Task ExecuteAsync_WithNoTrackedPackages_ShouldRenderEmptyTable()
    {
        // arrange
        var db = InMemoryDbContext.InitializeDatabase(
            [
                CollectionEntity.Create("Test-Collection-1").GetValue(),
            ]);
        var context = CommandAppContextFactory.CreateWithMemoryDb(db);
        context.Registrar.RegisterInstance(typeof(INuGetSearchClient), new FakeNuGetSearchClient(42));

        // act
        var result = await context.RunAsync(["get-current", "for-coll", "--collection-id", "1"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(Globals.S_OK, result.ExitCode);
        StringAssert.Contains(result.Output, "Success:");
        StringAssert.Contains(result.Output, "No packages exist");
        StringAssert.Contains(result.Output, "for 0 tracked packages");
    }

    [TestMethod]
    public async Task ExecuteAsync_WithMissingCollectionId_ReturnsError()
    {
        // arrange
        var db = InMemoryDbContext.InitializeDatabase(
            [
                CollectionEntity.Create("Test-Collection-1").GetValue(),
            ],
            [
                TrackedPackageEntity.Create("Test.Package.1", 1).GetValue(),
                TrackedPackageEntity.Create("Test.Package.2", 1).GetValue(),
                TrackedPackageEntity.Create("Test.Package.3", 1).GetValue(),
            ]);
        var context = CommandAppContextFactory.CreateWithMemoryDb(db);

        context.Registrar.RegisterInstance(typeof(INuGetSearchClient), new ErrorNuGetSearchClient());

        // act
        var result = await context.RunAsync(["get-current", "for-coll", "--collection-id", "404"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(Globals.E_FAIL, result.ExitCode);
        StringAssert.Contains(result.Output, "Error:");
        StringAssert.Contains(result.Output, "CollectionEntity with id=404 was not found");
    }

    [TestMethod]
    public async Task ExecuteAsync_WithClientError_ReturnsError()
    {
        // arrange
        var db = InMemoryDbContext.InitializeDatabase(
            [
                CollectionEntity.Create("Test-Collection-1").GetValue(),
            ],
            [
                TrackedPackageEntity.Create("Test.Package.1", 1).GetValue(),
                TrackedPackageEntity.Create("Test.Package.2", 1).GetValue(),
                TrackedPackageEntity.Create("Test.Package.3", 1).GetValue(),
            ]);
        var context = CommandAppContextFactory.CreateWithMemoryDb(db);

        context.Registrar.RegisterInstance(typeof(INuGetSearchClient), new ErrorNuGetSearchClient());

        // act
        var result = await context.RunAsync(["get-current", "for-coll", "--collection-id", "1"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(Globals.E_FAIL, result.ExitCode);
        StringAssert.Contains(result.Output, "Error:");
        StringAssert.Contains(result.Output, "Package with id=Test.Package.1 not found");
    }
}
