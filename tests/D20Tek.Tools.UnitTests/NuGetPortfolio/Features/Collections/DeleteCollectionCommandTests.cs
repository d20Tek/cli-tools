﻿using D20Tek.NuGet.Portfolio;
using D20Tek.NuGet.Portfolio.Domain;
using D20Tek.Tools.UnitTests.NuGetPortfolio.Fakes;

namespace D20Tek.Tools.UnitTests.NuGetPortfolio.Features.Collections;

[TestClass]
public class DeleteCollectionCommandTests
{

    [TestMethod]
    public async Task ExecuteAsync_WithMissingEntity_ReturnsNotFoundError()
    {
        // arrange
        var context = CommandAppContextFactory.CreateWithMemoryDb();

        // act
        var result = await context.RunAsync(["collection", "delete", "--id", "404"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(Globals.E_FAIL, result.ExitCode);
        StringAssert.Contains(result.Output, "Error:");
        StringAssert.Contains(result.Output, "not found");
    }

    [TestMethod]
    public async Task ExecuteAsync_WithCollectionId_ShouldRemoveEntity()
    {
        // arrange
        var db = InMemoryDbContext.InitializeDatabase(
            [
                CollectionEntity.Create("Test-Collection-1").GetValue(),
                CollectionEntity.Create("Test-Collection-2").GetValue(),
                CollectionEntity.Create("Test-Collection-3").GetValue()
            ]);
        var context = CommandAppContextFactory.CreateWithMemoryDb(db);

        // act
        var result = await context.RunAsync(["collection", "delete", "--id", "2"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(Globals.S_OK, result.ExitCode);
        StringAssert.Contains(result.Output, "Success:");
        StringAssert.Contains(result.Output, "Collection deleted: 'Test-Collection-2' [Id: 2]");
    }
}
