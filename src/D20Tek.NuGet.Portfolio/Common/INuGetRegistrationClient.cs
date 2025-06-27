namespace D20Tek.NuGet.Portfolio.Common;

public interface INuGetRegistrationClient
{
    Task<Result<int>> GetTotalDownloadsAsync(string packageId);
}
