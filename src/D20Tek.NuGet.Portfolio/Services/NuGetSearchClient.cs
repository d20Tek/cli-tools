using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Json;
using System.Text.Json;

namespace D20Tek.NuGet.Portfolio.Services;

internal class NuGetSearchClient : INuGetSearchClient
{
    private const string _dataNode = "data";
    private const string _downloadsProperty = "totalDownloads";
    private static readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

    private static string GetBaseUrl(string packageId) => $"query?q=packageid:{packageId}&prerelease=true";
    private static string GetCacheKey(string packageId) => $"NuGet.Package.{packageId.ToLowerInvariant()}";
    private static string PackageNotFound(string packageId) => $"Package with id '{packageId}' was not found.";

    private readonly HttpClient _httpClient;

    public NuGetSearchClient(HttpClient httpClient, IMemoryCache cache) => _httpClient = httpClient;

    public async Task<Result<int>> GetTotalDownloadsAsync(string packageId) =>
        await TryAsync.RunAsync<int>(async () =>
            await _cache.GetOrCreateAsync(GetCacheKey(packageId), async _ => 
                await GetServiceRequest(packageId).Pipe(json => CalculateTotalDownloads(json, packageId))));

    private Task<JsonElement> GetServiceRequest(string packageId) =>
        _httpClient.GetFromJsonAsync<JsonElement>(GetBaseUrl(packageId));

    public async Task<int> CalculateTotalDownloads(Task<JsonElement> jsonTask, string packageId) =>
        (await jsonTask).GetProperty(_dataNode)
                        .Pipe(data => data.GetArrayLength() > 0
                                          ? data[0].GetProperty(_downloadsProperty).GetInt32()
                                          : throw new InvalidOperationException(PackageNotFound(packageId)));
}
