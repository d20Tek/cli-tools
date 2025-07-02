namespace D20Tek.NuGet.Portfolio.Abstractions;

public interface INuGetSearchClient
{
    Task<Result<int>> GetTotalDownloadsAsync(string packageId);
}
