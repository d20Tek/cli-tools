using D20Tek.NuGet.Portfolio.Domain;

namespace D20Tek.Tools.UnitTests.NuGetPortfolio;

[TestClass]
public class DomainEntityTests
{
    [TestMethod]
    public void CollectionEntity_Creation_ShouldSetAllProperties()
    {
        // arrange
        List<TrackedPackageEntity> packages =
        [
            TrackedPackageEntity.Create("Test.Package.1", 1).GetValue()
        ];

        // act
        var result = CollectionEntity.Create("TestCat");
        var coll = result.GetValue();
        coll.TrackedPackages = packages;

        // assert
        Assert.AreEqual(0, coll.Id);
        Assert.AreEqual("TestCat", coll.Name);
        Assert.HasCount(1, coll.TrackedPackages);
    }

    [TestMethod]
    public void TrackedPackageEntity_Creation_ShouldSetAllProperties()
    {
        // arrange

        // act
        var result = TrackedPackageEntity.Create("Test.Package.1", 1);
        var tracked = result.GetValue();

        // assert
        Assert.AreEqual(0, tracked.Id);
        Assert.AreEqual("Test.Package.1", tracked.PackageId);
        Assert.AreEqual(1, tracked.CollectionId);
        Assert.AreEqual(DateOnly.FromDateTime(DateTime.Now.ToLocalTime()), tracked.DateAdded);
    }

    [TestMethod]
    public void PackageSnapshotEntity_Creation_ShouldSetAllProperties()
    {
        // arrange
        var pkg = TrackedPackageEntity.Create("Test.Package.1", 1).GetValue();

        // act
        var result = PackageSnapshotEntity.Create(12, pkg);

        // assert
        Assert.AreEqual(0, result.Id);
        Assert.AreEqual(0, result.TrackedPackageId);
        Assert.AreEqual(12, result.Downloads);
        Assert.AreEqual(DateOnly.FromDateTime(DateTime.Now.ToLocalTime()), result.SnapshotDate);
    }
}
