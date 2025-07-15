using D20Tek.NuGet.Portfolio;
using D20Tek.NuGet.Portfolio.Domain;
using D20Tek.Tools.UnitTests.NuGetPortfolio.Fakes;

namespace D20Tek.Tools.UnitTests.NuGetPortfolio.Features.Snapshots;

[TestClass]
public class DeleteSnapshotsCommandTests
{
    [TestMethod]
    public async Task ExecuteAsync_WithMissingCollectionId_ReturnsNotFoundError()
    {
        // arrange
        var context = CommandAppContextFactory.CreateWithMemoryDb();

        // act
        var result = await context.RunAsync(["snapshot", "delete", "-c", "404", "-d", "7/15/2025"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(Globals.E_FAIL, result.ExitCode);
        StringAssert.Contains(result.Output, "Error:");
        StringAssert.Contains(result.Output, "not found");
    }

    [TestMethod]
    public async Task ExecuteAsync_WithMissingSnapshotDate_ShouldRemoveNoEntities()
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
                TrackedPackageEntity.Create("Test.Package.3", 1).GetValue(),
                TrackedPackageEntity.Create("Test.Package.4", 2).GetValue(),
            ],
            [
                PackageSnapshotEntity.Create(10, pkg)
            ]);
        var context = CommandAppContextFactory.CreateWithMemoryDb(db);

        // act
        var result = await context.RunAsync(["snapshot", "delete", "-c", "2", "-d", "7/15/2000"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(Globals.S_OK, result.ExitCode);
        StringAssert.Contains(result.Output, "Success:");
        StringAssert.Contains(result.Output, "0 snapshots deleted");
        Assert.AreEqual(1, db.PackageSnapshots.Count());
    }

    [TestMethod]
    public async Task ExecuteAsync_FromOtherCollectionWithSnapshotDate_ShouldRemoveNoEntities()
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
                TrackedPackageEntity.Create("Test.Package.3", 1).GetValue(),
                TrackedPackageEntity.Create("Test.Package.4", 2).GetValue(),
            ],
            [
                PackageSnapshotEntity.Create(10, pkg)
            ]);
        var context = CommandAppContextFactory.CreateWithMemoryDb(db);

        var today = DateOnly.FromDateTime(DateTime.Today).ToShortDateString();

        // act
        var result = await context.RunAsync(["snapshot", "delete", "-c", "1", "-d", today]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(Globals.S_OK, result.ExitCode);
        StringAssert.Contains(result.Output, "Success:");
        StringAssert.Contains(result.Output, "0 snapshots deleted");
        Assert.AreEqual(1, db.PackageSnapshots.Count());
    }

    [TestMethod]
    public async Task ExecuteAsync_WithSnapshotDate_ShouldRemoveEntities()
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
                TrackedPackageEntity.Create("Test.Package.3", 1).GetValue(),
                TrackedPackageEntity.Create("Test.Package.4", 2).GetValue(),
            ],
            [
                PackageSnapshotEntity.Create(10, pkg)
            ]);
        var context = CommandAppContextFactory.CreateWithMemoryDb(db);

        var today = DateOnly.FromDateTime(DateTime.Today).ToShortDateString();

        // act
        var result = await context.RunAsync(["snapshot", "delete", "-c", "2", "-d", today]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(Globals.S_OK, result.ExitCode);
        StringAssert.Contains(result.Output, "Success:");
        StringAssert.Contains(result.Output, "1 snapshots deleted");
        Assert.AreEqual(0, db.PackageSnapshots.Count());
    }
}
