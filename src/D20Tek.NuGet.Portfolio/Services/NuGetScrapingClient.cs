using D20Tek.NuGet.Portfolio.Abstractions;
using HtmlAgilityPack;
using Microsoft.Extensions.Caching.Memory;
using System.Text.RegularExpressions;

namespace D20Tek.NuGet.Portfolio.Services;

internal class NuGetScrapingClient : INuGetSearchClient
{
    private const string _versionHistoryPath = "//div[@id='version-history']//table//tbody//tr";
    private const string _cellPath = "td";
    private static readonly Error _invalidVersionHistory =
        Error.Invalid("VersionHistory.Invalid", "Could not find the expected version-history table.");
    private static Error _downloadsNotFound(string packageId) =>
        Error.Invalid("Downloads.NotFound", $"No downloads were found for this package id: {packageId}.");
    private static string GetDownloadUrl(string packageId) => $"https://www.nuget.org/packages/{packageId}";

    private static readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());
    private readonly HttpClient _httpClient;

    public NuGetScrapingClient(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<Result<long>> GetTotalDownloadsAsync(string packageId)
    {
        return (await GetServiceRequest(packageId)).Bind(rows => CalculateTotalDownloads(rows, packageId));
    }

    private async Task<Result<HtmlNodeCollection>> GetServiceRequest(string packageId)
    {
        var url = GetDownloadUrl(packageId);
        var html = await _httpClient.GetStringAsync(url);

        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        // XPath that matches rows under the version history table
        var rows = doc.DocumentNode.SelectNodes(_versionHistoryPath);
        if (rows == null || rows.Count == 0)
            return Result<HtmlNodeCollection>.Failure(_invalidVersionHistory);

        return rows;
    }

    public Result<long> CalculateTotalDownloads(HtmlNodeCollection rows, string packageId)
    {
        long total = 0;

        foreach (var row in rows)
        {
            var cells = row.SelectNodes(_cellPath);

            if (cells?.Count >= 2)
            {
                string downloadText = cells[1].InnerText.Trim();
                downloadText = Regex.Replace(downloadText, @"[^\d]", "");

                if (long.TryParse(downloadText, out var count))
                {
                    total += count;
                }
            }
        }

        return total > 0 ? total : Result<long>.Failure(_downloadsNotFound(packageId));
    }
}
