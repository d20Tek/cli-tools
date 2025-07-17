using D20Tek.NuGet.Portfolio;
using D20Tek.NuGet.Portfolio.Domain;
using D20Tek.Tools.UnitTests.NuGetPortfolio.Fakes;

namespace D20Tek.Tools.UnitTests.NuGetPortfolio.Features.Snapshots;

[TestClass]
public class ListTodayByCollectionCommandTests
{

    [TestMethod]
    public async Task ExecuteAsync_WithSnapshotsToday_ShouldShowDownloadDetails()
    {
        // arrange
        var pkg1 = TrackedPackageEntity.Create("Test.Package.1", 1).GetValue();
        var pkg2 = TrackedPackageEntity.Create("Test.Package.2", 1).GetValue();
        var pkg3 = TrackedPackageEntity.Create("Test.Package.3", 1).GetValue();
        var db = InMemoryDbContext.InitializeDatabase(
            [
                CollectionEntity.Create("Test-Collection-1").GetValue(),
                CollectionEntity.Create("Test-Collection-2").GetValue(),
            ],
            [
                pkg1,
                pkg2,
                pkg3,
                TrackedPackageEntity.Create("Test.Package.4", 2).GetValue(),
            ],
            [
                PackageSnapshotEntity.Create(10, pkg1),
                PackageSnapshotEntity.Create(20, pkg2),
                PackageSnapshotEntity.Create(30, pkg3)
            ]);
        var context = CommandAppContextFactory.CreateWithMemoryDb(db);

        // act
        var result = await context.RunAsync(["snapshot", "today", "-c", "1"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(Globals.S_OK, result.ExitCode);
        StringAssert.Contains(result.Output, "Success:");
        StringAssert.Contains(result.Output, "Test.Package.1");
        StringAssert.Contains(result.Output, "10");
        StringAssert.Contains(result.Output, "Test.Package.2");
        StringAssert.Contains(result.Output, "20");
        StringAssert.Contains(result.Output, "Test.Package.3");
        StringAssert.Contains(result.Output, "30");
        StringAssert.Contains(result.Output, "snapshots for 3 tracked packages");
    }

    [TestMethod]
    public async Task ExecuteAsync_WithMissingSnapshotsToday_ShouldShowNoResults()
    {
        // arrange
        var pkg1 = TrackedPackageEntity.Create("Test.Package.1", 1).GetValue();
        var pkg2 = TrackedPackageEntity.Create("Test.Package.2", 1).GetValue();
        var pkg3 = TrackedPackageEntity.Create("Test.Package.3", 1).GetValue();
        var db = InMemoryDbContext.InitializeDatabase(
            [
                CollectionEntity.Create("Test-Collection-1").GetValue(),
                CollectionEntity.Create("Test-Collection-2").GetValue(),
            ],
            [
                pkg1,
                pkg2,
                pkg3,
                TrackedPackageEntity.Create("Test.Package.4", 2).GetValue(),
            ],
            [
                PackageSnapshotEntity.Create(10, pkg1),
                PackageSnapshotEntity.Create(20, pkg2),
                PackageSnapshotEntity.Create(30, pkg3)
            ]);
        var context = CommandAppContextFactory.CreateWithMemoryDb(db);

        // act
        var result = await context.RunAsync(["snapshot", "today", "-c", "2"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(Globals.S_OK, result.ExitCode);
        StringAssert.Contains(result.Output, "Success:");
        StringAssert.Contains(result.Output, "No package downloads exist");
        StringAssert.Contains(result.Output, "snapshots for 0 tracked packages");
    }
}
