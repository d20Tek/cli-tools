using D20Tek.NuGet.Portfolio.Persistence;
using D20Tek.Tools.UnitTests.NuGetPortfolio.Fakes;
using Microsoft.Data.Sqlite;

namespace D20Tek.Tools.UnitTests.NuGetPortfolio.Persistence;

[TestClass]
public class AppDbContextTests
{

    [TestMethod]
    public void ApplyMigrations_WithTempDatabaseFile_ShouldRunMigrations()
    {
        // arrange
        string conn = $"Data Source={Guid.NewGuid()}.db";
        var dbContext = new AppContextFactory(conn).CreateDbContext([]);

        // act
        dbContext.ApplyMigrations();

        // assert
    }


    [TestMethod]
    public void ApplyMigrations_WithMemoryDb_ShouldThrowException()
    {
        // arrange
        var dbContext = InMemoryDbContext.Create();

        // act - assert
        Assert.Throws<SqliteException>(dbContext.ApplyMigrations);
    }

}