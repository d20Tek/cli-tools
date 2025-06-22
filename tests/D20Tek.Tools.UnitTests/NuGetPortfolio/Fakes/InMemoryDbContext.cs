using D20Tek.Functional;
using D20Tek.NuGet.Portfolio.Domain;
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

    public static AppDbContext InitializeDatabase(CollectionEntity[] entities) =>
        InMemoryDbContext.Create().ToIdentity()
                         .Iter(db => db.Collections.AddRange(entities))
                         .Iter(db => db.SaveChanges());

    public static AppDbContext InitializeDatabase(CollectionEntity[] entities, TrackedPackageEntity[] packages) =>
        InMemoryDbContext.Create().ToIdentity()
                         .Iter(db => db.Collections.AddRange(entities))
                         .Iter(db => db.TrackedPackages.AddRange(packages))
                         .Iter(db => db.SaveChanges());
}
