using D20Tek.NuGet.Portfolio.Abstractions;
using D20Tek.NuGet.Portfolio.Features.Helpers;
using D20Tek.NuGet.Portfolio.Persistence;

namespace D20Tek.NuGet.Portfolio.Features.PackageDownloads;

internal sealed class GetDownloadsAllPackagesCommand(
    IAnsiConsole console,
    AppDbContext dbContext,
    INuGetSearchClient client) : AsyncCommand
{
    private readonly IAnsiConsole _console = console;
    private readonly AppDbContext _dbContext = dbContext;
    private readonly INuGetSearchClient _client = client;

    public override async Task<int> ExecuteAsync(CommandContext context, CancellationToken token) =>
        await _dbContext.GetAllTrackedPackages()
                .Pipe(_client.RetrieveDownloadSnapshots)
                .IterAsync(_console.RenderDownloadSnapshots)
                .RenderAsync(_console, s => $"Retrieved downloads for {s.Length} tracked packages.");
}
