using D20Tek.Functional;
using D20Tek.NuGet.Portfolio.Abstractions;

namespace D20Tek.Tools.UnitTests.NuGetPortfolio.Fakes;

internal class FakeNuGetSearchClient : INuGetSearchClient
{
    private readonly int _downloadCount;

    public FakeNuGetSearchClient(int downloadCount) => _downloadCount = downloadCount;

    public Task<Result<long>> GetTotalDownloadsAsync(string packageId) => 
        Task.FromResult<Result<long>>(_downloadCount);
}

internal class ErrorNuGetSearchClient : INuGetSearchClient
{
    public Task<Result<long>> GetTotalDownloadsAsync(string packageId) =>
        Task.FromResult(
            Result<long>.Failure(Error.NotFound("Package.NotFound", $"Package with id={packageId} not found.")));
}
