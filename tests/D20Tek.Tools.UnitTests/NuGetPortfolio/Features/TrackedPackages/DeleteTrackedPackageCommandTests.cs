using D20Tek.NuGet.Portfolio;
using D20Tek.NuGet.Portfolio.Domain;
using D20Tek.Tools.UnitTests.NuGetPortfolio.Fakes;

namespace D20Tek.Tools.UnitTests.NuGetPortfolio.Features.TrackedPackages;

[TestClass]
public class DeleteTrackedPackageCommandTests
{
    [TestMethod]
    public async Task ExecuteAsync_WithMissingEntity_ReturnsNotFoundError()
    {
        // arrange
        var context = CommandAppContextFactory.CreateWithMemoryDb();

        // act
        var result = await context.RunAsync(["package", "delete", "--id", "404"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(Globals.E_FAIL, result.ExitCode);
        Assert.Contains("Error:", result.Output);
        Assert.Contains("not found", result.Output);
    }

    [TestMethod]
    public async Task ExecuteAsync_WithPackageId_ShouldRemoveEntity()
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
        var result = await context.RunAsync(["package", "delete", "--id", "3"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(Globals.S_OK, result.ExitCode);
        Assert.Contains("Success:", result.Output);
        Assert.Contains("Tracked package deleted: 'Test.Package.3' [Id: 3]", result.Output);
    }
}
