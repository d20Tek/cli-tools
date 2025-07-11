using D20Tek.NuGet.Portfolio.Abstractions;
using D20Tek.NuGet.Portfolio.Persistence;

namespace D20Tek.NuGet.Portfolio.Features.PackageDownloads;

internal sealed class GetDownloadsAllPackagesCommand : AsyncCommand
{
    private readonly IAnsiConsole _console;
    private readonly AppDbContext _dbContext;
    private readonly INuGetSearchClient _client;

    public GetDownloadsAllPackagesCommand(
        IAnsiConsole console,
        AppDbContext dbContext,
        INuGetSearchClient client) =>
        (_console, _dbContext, _client) = (console, dbContext, client);

    public override async Task<int> ExecuteAsync(CommandContext context) =>
        await _dbContext.GetAllTrackedPackages()
                .Pipe(p => _client.RetrieveDownloadSnapshots(p))
                .IterAsync(s => _console.RenderDownloadSnapshots(s))
                .RenderAsync(_console, s => $"Retrieved downloads for {s.Length} tracked packages.");
}
