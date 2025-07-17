using D20Tek.NuGet.Portfolio;
using D20Tek.NuGet.Portfolio.Abstractions;
using D20Tek.NuGet.Portfolio.Domain;
using D20Tek.Tools.UnitTests.NuGetPortfolio.Fakes;

namespace D20Tek.Tools.UnitTests.NuGetPortfolio.Features.Snapshots;

[TestClass]
public class AddSnapshotCommandTests
{
    [TestMethod]
    public async Task ExecuteAsync_WithCollectionTrackedPackages_ShouldSaveSnapshot()
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
        var result = await context.RunAsync(["snapshot", "add", "--collection-id", "1"]);

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
        Assert.AreEqual(2, db.PackageSnapshots.Count());
        Assert.IsTrue(db.PackageSnapshots.Any(x => x.TrackedPackageId == 1));
        Assert.IsTrue(db.PackageSnapshots.Any(x => x.TrackedPackageId == 2));
    }

    [TestMethod]
    public async Task ExecuteAsync_WithNoTrackedPackages_ShouldSaveNoSnapshots()
    {
        // arrange
        var db = InMemoryDbContext.InitializeDatabase(
            [
                CollectionEntity.Create("Test-Collection-1").GetValue(),
            ]);
        var context = CommandAppContextFactory.CreateWithMemoryDb(db);
        context.Registrar.RegisterInstance(typeof(INuGetSearchClient), new FakeNuGetSearchClient(42));

        // act
        var result = await context.RunAsync(["snapshot", "add", "--collection-id", "1"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(Globals.S_OK, result.ExitCode);
        StringAssert.Contains(result.Output, "Success:");
        StringAssert.Contains(result.Output, "No package downloads exist");
        StringAssert.Contains(result.Output, "for 0 tracked packages");
        Assert.AreEqual(0, db.PackageSnapshots.Count());
    }

    [TestMethod]
    public async Task ExecuteAsync_WithExistingSnapshot_ShouldSaveSnapshot()
    {
        // arrange
        var pkg = TrackedPackageEntity.Create("Test.Package.3", 2).GetValue();
        var db = InMemoryDbContext.InitializeDatabase(
            [
                CollectionEntity.Create("Test-Collection-1").GetValue(),
                CollectionEntity.Create("Test-Collection-2").GetValue(),
            ],
            [
                TrackedPackageEntity.Create("Test.Package.1", 1).GetValue(),
                TrackedPackageEntity.Create("Test.Package.2", 1).GetValue(),
                pkg,
            ],
            [
                PackageSnapshotEntity.Create(10, pkg)
            ]);
        var context = CommandAppContextFactory.CreateWithMemoryDb(db);

        context.Registrar.RegisterInstance(typeof(INuGetSearchClient), new FakeNuGetSearchClient(42));

        // act
        var result = await context.RunAsync(["snapshot", "add", "--collection-id", "2"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(Globals.S_OK, result.ExitCode);
        StringAssert.Contains(result.Output, "Success:");
        StringAssert.Contains(result.Output, "42");
        StringAssert.Contains(result.Output, "for 1 tracked packages");
        Assert.AreEqual(1, db.PackageSnapshots.Count());
        Assert.IsTrue(db.PackageSnapshots.Any(x => x.TrackedPackageId == 3));
        Assert.AreEqual(42, db.PackageSnapshots.First(x => x.TrackedPackageId == 3).Downloads);
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
        var result = await context.RunAsync(["snapshot", "add", "--collection-id", "404"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(Globals.E_FAIL, result.ExitCode);
        StringAssert.Contains(result.Output, "Error:");
        StringAssert.Contains(result.Output, "CollectionEntity with id=404 was not found");
        Assert.AreEqual(0, db.PackageSnapshots.Count());
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
        var result = await context.RunAsync(["snapshot", "add", "--collection-id", "1"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(Globals.E_FAIL, result.ExitCode);
        StringAssert.Contains(result.Output, "Error:");
        StringAssert.Contains(result.Output, "Package with id=Test.Package.1 not found");
        Assert.AreEqual(0, db.PackageSnapshots.Count());
    }
}
