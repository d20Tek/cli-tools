using D20Tek.NuGet.Portfolio.Domain;
using D20Tek.NuGet.Portfolio.Persistence;

namespace D20Tek.NuGet.Portfolio.Features.PackageDownloads;

internal class GetDownloadsAllPackagesCommand : AsyncCommand
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
        await GetTrackedPackages()
                .Pipe(p => RetrieveDownloadSnapshots(p))
                .IterAsync(s => RenderDownloadSnapshots(s))
                .RenderAsync(_console, s => $"Retrieved downloads for {s.Length} tracked packages.");

    public TrackedPackageEntity[] GetTrackedPackages() => [.. _dbContext.TrackedPackages];

    private async Task<Result<PackageSnapshotEntity[]>> RetrieveDownloadSnapshots(TrackedPackageEntity[] packages)
    {
        List<PackageSnapshotEntity> result = [];
        foreach (var package in packages)
        {
            var downloads = await _client.GetTotalDownloadsAsync(package.PackageId);
            if (downloads.IsFailure) return downloads.MapErrors<PackageSnapshotEntity[]>();

            result.Add(PackageSnapshotEntity.Create(downloads.GetValue(), package));
        }

        return result.ToArray();
    }

    private Task RenderDownloadSnapshots(PackageSnapshotEntity[] snapshots) =>
        _console.ToIdentity()
                .Iter(c => c.WriteLine())
                .Iter(c => c.CommandHeader().Render("All package downloads"))
                .Iter(c => c.Write(DownloadsTableBuilder.Create()
                                                        .WithHeader()
                                                        .WithRows(snapshots)
                                                        .Build()))
                .Map(_ => Task.CompletedTask);
}
