using D20Tek.NuGet.Portfolio.Domain;

namespace D20Tek.NuGet.Portfolio.Features.PackageDownloads;

internal static class DownloadsHelper
{
    public static async Task<Result<PackageSnapshotEntity[]>> RetrieveDownloadSnapshots(
        this INuGetSearchClient client,
        TrackedPackageEntity[] packages)
    {
        List<PackageSnapshotEntity> result = [];
        foreach (var package in packages)
        {
            var downloads = await client.GetTotalDownloadsAsync(package.PackageId);
            if (downloads.IsFailure) return downloads.MapErrors<PackageSnapshotEntity[]>();

            result.Add(PackageSnapshotEntity.Create(downloads.GetValue(), package));
        }

        return result.ToArray();
    }

    public static Task RenderDownloadSnapshots(this IAnsiConsole console, PackageSnapshotEntity[] snapshots)
    {
        console.RenderTableWithTitle(
                   "Collection package downloads",
                   DownloadsTableBuilder.Create()
                                        .WithHeader()
                                        .WithRows(snapshots)
                                        .WithTotals(snapshots)
                                        .Build());
        return Task.CompletedTask;
    }
}
