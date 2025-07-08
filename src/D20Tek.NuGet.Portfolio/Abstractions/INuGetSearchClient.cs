namespace D20Tek.NuGet.Portfolio.Abstractions;

public interface INuGetSearchClient
{
    Task<Result<long>> GetTotalDownloadsAsync(string packageId);
}
