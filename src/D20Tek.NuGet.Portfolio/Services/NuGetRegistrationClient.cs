using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Json;
using System.Text.Json;

namespace D20Tek.NuGet.Portfolio.Services;

internal class NuGetRegistrationClient : INuGetRegistrationClient
{
    private const string _itemsNode = "items";
    private const string _catalogNode = "catalogEntry";
    private const string _downloadsProperty = "downloads";

    private static string GetBaseUrl(string packageId) => $"{packageId.ToLowerInvariant()}/index.json";
    private static string GetCacheKey(string packageId) => $"NuGet.Package.{packageId.ToLowerInvariant()}";
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;

    public NuGetRegistrationClient(HttpClient httpClient, IMemoryCache cache) =>
        (_httpClient, _cache) = (httpClient, cache);

    public async Task<Result<int>> GetTotalDownloadsAsync(string packageId) =>
        await TryAsync.RunAsync<int>(async () =>
            await _cache.GetOrCreateAsync(GetCacheKey(packageId), async _ => 
                await GetServiceRequest(packageId).Pipe(json => CalculateTotalDownloads(json))));

    private Task<JsonElement> GetServiceRequest(string packageId) => 
        _httpClient.GetFromJsonAsync<JsonElement>(GetBaseUrl(packageId));

    private async Task<int> CalculateTotalDownloads(Task<JsonElement> json) =>
        (await json)
            .GetProperty(_itemsNode)
            .EnumerateArray()
            .Where(page => page.TryGetProperty(_itemsNode, out _))
            .SelectMany(page => page.GetProperty(_itemsNode).EnumerateArray())
            .Select(item => item
                .GetProperty(_catalogNode)
                .GetProperty(_downloadsProperty)
                .GetInt32())
            .Sum();
}
