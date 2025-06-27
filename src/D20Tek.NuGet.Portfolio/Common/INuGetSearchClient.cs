namespace D20Tek.NuGet.Portfolio.Common;

public interface INuGetSearchClient
{
    Task<Result<int>> GetTotalDownloadsAsync(string packageId);
}
