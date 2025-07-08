using D20Tek.NuGet.Portfolio.Abstractions;
using HtmlAgilityPack;
using Microsoft.Extensions.Caching.Memory;
using System.Text.RegularExpressions;

namespace D20Tek.NuGet.Portfolio.Services;

internal class NuGetScrapingClient : INuGetSearchClient
{
    private const string _versionHistoryPath = "//div[@id='version-history']//table//tbody//tr";
    private const string _cellPath = "td";
    private const string _digitRegex = @"[^\d]";
    private const string _versionHistoryError = "Could not find the expected version-history table";
    private static string _downloadsNotFound(string packageId) =>
        $"No downloads were found for this package id: {packageId}.";
    private static string GetDownloadUrl(string packageId) => $"packages/{packageId}";
    private static string GetCacheKey(string packageId) => $"NuGet.Package.{packageId.ToLowerInvariant()}";

    private static readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());
    private readonly HttpClient _httpClient;

    public NuGetScrapingClient(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<Result<long>> GetTotalDownloadsAsync(string packageId) =>
        await TryAsync.RunAsync<long>(async () =>
            await _cache.GetOrCreateAsync(GetCacheKey(packageId), async _ =>
                (await GetVersionHistoryRows(packageId)).Pipe(rows => CalculateTotalDownloads(rows, packageId))));

    private async Task<HtmlNodeCollection> GetVersionHistoryRows(string packageId) =>
        (await LoadHtml(GetDownloadUrl(packageId)))
            .Pipe(doc => doc.DocumentNode.SelectNodes(_versionHistoryPath))
            .Pipe(rows => (rows == null || rows.Count == 0) ?
                          throw new InvalidOperationException(_versionHistoryError) :
                          rows);

    private async Task<HtmlDocument> LoadHtml(string url) =>
        await new HtmlDocument().ToIdentity()
                                .IterAsync(async doc => doc.LoadHtml(await _httpClient.GetStringAsync(url)));

    private long CalculateTotalDownloads(HtmlNodeCollection rows, string packageId) =>
        GetTotalFromRows(rows).Pipe(total => total > 0
                                           ? total
                                           : throw new InvalidOperationException(_downloadsNotFound(packageId)));
    private long GetTotalFromRows(HtmlNodeCollection rows) =>
        rows.Select(row => row.SelectNodes(_cellPath))
            .Where(cells => cells is not null && cells.Count >= 2)
            .Select(c => ParseCell(c![1]))
            .Sum();

    private static long ParseCell(HtmlNode node) =>
        Regex.Replace(node.InnerText.Trim(), _digitRegex, string.Empty)
             .Pipe(text => long.Parse(text));
}
