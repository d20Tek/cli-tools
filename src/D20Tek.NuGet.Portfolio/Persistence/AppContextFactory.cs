using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Logging.Abstractions;
using System.Diagnostics.CodeAnalysis;

namespace D20Tek.NuGet.Portfolio.Persistence;

internal sealed class AppContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    private readonly string _connection;

    [ExcludeFromCodeCoverage]
    public AppContextFactory(string connection) => _connection = connection;

    public AppContextFactory() => _connection = "Data Source=nu-port.db";

    public AppDbContext CreateDbContext(string[] args) =>
        new(new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .Options,
            new NullLogger<AppDbContext>());
}
