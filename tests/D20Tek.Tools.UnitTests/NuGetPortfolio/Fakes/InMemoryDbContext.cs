using D20Tek.NuGet.Portfolio.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace D20Tek.Tools.UnitTests.NuGetPortfolio.Fakes;

internal static class InMemoryDbContext
{
    public static AppDbContext Create()
    {
        // 1. Create and open a SQLite in-memory connection
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        // 2. Configure DbContext to use the open connection
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        var context = new AppDbContext(options, new NullLogger<AppDbContext>());

        // 3. Create schema
        context.Database.EnsureCreated();
        return context;
    }
}
