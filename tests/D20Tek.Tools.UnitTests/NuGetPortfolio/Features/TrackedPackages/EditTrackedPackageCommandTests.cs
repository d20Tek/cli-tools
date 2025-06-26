﻿using D20Tek.NuGet.Portfolio;
using D20Tek.NuGet.Portfolio.Domain;
using D20Tek.NuGet.Portfolio.Persistence;
using D20Tek.Tools.UnitTests.NuGetPortfolio.Fakes;

namespace D20Tek.Tools.UnitTests.NuGetPortfolio.Features.TrackedPackages;

[TestClass]
public class EditTrackedPackageCommandTests
{
    [TestMethod]
    public async Task ExecuteAsync_WithValidProperties_UpdatesCollectionEntity()
    {
        // arrange
        var context = CommandAppContextFactory.CreateWithMemoryDb(CreateDatabaseWithPackages());

        // act
        var result = await context.RunAsync(["package", "edit", "--id", "2", "--package-id", "Test.Package.2.Updated", "--collection-id", "2"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(Globals.S_OK, result.ExitCode);
        StringAssert.Contains(result.Output, "Success:");
        StringAssert.Contains(result.Output, "'Test.Package.2.Updated'");
    }

    [TestMethod]
    public async Task ExecuteAsync_WithMissingId_ReturnsNotFoundError()
    {
        // arrange
        var context = CommandAppContextFactory.CreateWithMemoryDb();

        // act
        var result = await context.RunAsync(["package", "edit", "--id", "999", "--package-id", "Test.Package.2.Updated", "--collection-id", "2"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(Globals.E_FAIL, result.ExitCode);
        StringAssert.Contains(result.Output, "Error:");
        StringAssert.Contains(result.Output, "not found");
    }

    [TestMethod]
    public async Task ExecuteAsync_WithEmptyProperties_UpdatesPackageEntity()
    {
        // arrange
        var context = CommandAppContextFactory.CreateWithMemoryDb(CreateDatabaseWithPackages());
        context.Console.TestInput.PushTextWithEnter("2");
        context.Console.TestInput.PushTextWithEnter("Test.Package.2.Interactive");
        context.Console.TestInput.PushTextWithEnter("1");

        // act
        var result = await context.RunAsync(["package", "edit"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(Globals.S_OK, result.ExitCode);
        StringAssert.Contains(result.Output, "Success:");
        StringAssert.Contains(result.Output, "'Test.Package.2.Interactive'");
    }

    [TestMethod]
    public async Task ExecuteAsync_WithMissingCollectionId_ReturnsNotFoundError()
    {
        // arrange
        var context = CommandAppContextFactory.CreateWithMemoryDb(CreateDatabaseWithPackages());

        // act
        var result = await context.RunAsync(["package", "edit", "--id", "2", "--package-id", "Test.Package.2.Updated", "--collection-id", "99"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(Globals.E_FAIL, result.ExitCode);
        StringAssert.Contains(result.Output, "Error:");
        StringAssert.Contains(result.Output, "CollectionEntity");
        StringAssert.Contains(result.Output, "not found");
    }

    private AppDbContext CreateDatabaseWithPackages() =>
        InMemoryDbContext.InitializeDatabase(
            [
                CollectionEntity.Create("Default").GetValue(),
                CollectionEntity.Create("OtherCollection").GetValue(),
            ],
            [
                TrackedPackageEntity.Create("Test.Package.1", 1).GetValue(),
                TrackedPackageEntity.Create("Test.Package.2", 1).GetValue(),
                TrackedPackageEntity.Create("Test.Package.3", 1).GetValue()
            ]);
}
