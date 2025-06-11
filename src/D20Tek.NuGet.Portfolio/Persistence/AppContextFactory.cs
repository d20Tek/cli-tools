using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Logging.Abstractions;
using System.Diagnostics.CodeAnalysis;

namespace D20Tek.NuGet.Portfolio.Persistence;

internal sealed class AppContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    private readonly string _connection;

    [ExcludeFromCodeCoverage]
    public AppContextFactory(string? connection = null)
    {
        _connection = connection ?? "Data Source=nu-port.db";
    }

    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlite(_connection);

        return new AppDbContext(optionsBuilder.Options, new NullLogger<AppDbContext>());
    }
}
