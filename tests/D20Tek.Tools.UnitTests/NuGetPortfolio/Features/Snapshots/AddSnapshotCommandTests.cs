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
        Assert.Contains("Success:", result.Output);
        Assert.Contains("Test.Package.1", result.Output);
        Assert.Contains("Test.Package.2", result.Output);
        Assert.DoesNotContain("Test.Package.3", result.Output);
        Assert.Contains("42", result.Output);
        Assert.Contains("84", result.Output);
        Assert.Contains("for 2 tracked packages", result.Output);
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
        Assert.Contains("Success:", result.Output);
        Assert.Contains("No package downloads exist", result.Output);
        Assert.Contains("for 0 tracked packages", result.Output);
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
        Assert.Contains("Success:", result.Output);
        Assert.Contains("42", result.Output);
        Assert.Contains("for 1 tracked packages", result.Output);
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
        Assert.Contains("Error:", result.Output);
        Assert.Contains("CollectionEntity with id=404 was not found", result.Output);
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
        Assert.Contains("Error:", result.Output);
        Assert.Contains("Package with id=Test.Package.1 not found", result.Output);
        Assert.AreEqual(0, db.PackageSnapshots.Count());
    }
}
