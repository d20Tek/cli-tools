using D20Tek.NuGet.Portfolio.Services;
using D20Tek.Tools.UnitTests.NuGetPortfolio.Fakes;
using Microsoft.Extensions.Caching.Memory;

namespace D20Tek.Tools.UnitTests.NuGetPortfolio.Services;

[TestClass]
public class NuGetRegistrationClientTests
{

    [TestMethod]
    public async Task GetTotalDownloadsAsync_WithPackageId_ReturnsTotal()
    {
        // arrange
        string testPackageId = "TestPackage";
        string jsonResponse = """
        {
          "@context": {
            "@vocab": "http://schema.nuget.org/schema#",
            "@base": "https://api.nuget.org/v3/registration5-semver1/"
          },
          "totalHits": 1,
          "data": [
            {
              "@id": "https://api.nuget.org/v3/registration5-semver1/create-guid/index.json",
              "@type": "Package",
              "registration": "https://api.nuget.org/v3/registration5-semver1/create-guid/index.json",
              "id": "create-guid",
              "title": "Create Guid",
              "totalDownloads": 600,
              "verified": false,
              "packageTypes": [
                {
                  "name": "DotnetTool"
                }
              ],
              "versions": [],
              "vulnerabilities": []
            }
          ]
        }
        """;

        var handler = new FakeHttpMessageHandler(jsonResponse);
        var http = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api-v2v3search-0.nuget.org/")
        };
        var cache = new MemoryCache(new MemoryCacheOptions());
        var client = new NuGetSearchClient(http, cache);

        // act
        var result = await client.GetTotalDownloadsAsync(testPackageId);

        // assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(600, result.GetValue());
    }

    [TestMethod]
    public async Task GetTotalDownloadsAsync_WithHttpException_ReturnsError()
    {
        // arrange
        string testPackageId = "TestPackage";
        var handler = new ErrorHttpMessageHandler();
        var http = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api-v2v3search-0.nuget.org/")
        };
        var cache = new MemoryCache(new MemoryCacheOptions());
        var client = new NuGetSearchClient(http, cache);

        // act
        var result = await client.GetTotalDownloadsAsync(testPackageId);

        // assert
        Assert.IsTrue(result.IsFailure);
        var error = result.GetErrors().FirstOrDefault();
        Assert.AreEqual("General.Exception", error.Code);
    }

    [TestMethod]
    public async Task GetTotalDownloadsAsync_WithMissingPackageId_ReturnsFailure()
    {
        // arrange
        string testPackageId = "TestPackage";
        string jsonResponse = """
        {
          "@context": {
            "@vocab": "http://schema.nuget.org/schema#",
            "@base": "https://api.nuget.org/v3/registration5-semver1/"
          },
          "totalHits": 0,
          "data": []
        }
        """;

        var handler = new FakeHttpMessageHandler(jsonResponse);
        var http = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api-v2v3search-0.nuget.org/")
        };
        var cache = new MemoryCache(new MemoryCacheOptions());
        var client = new NuGetSearchClient(http, cache);

        // act
        var result = await client.GetTotalDownloadsAsync(testPackageId);

        // assert
        Assert.IsTrue(result.IsFailure);
        var error = result.GetErrors().FirstOrDefault();
        Assert.AreEqual("General.Exception", error.Code);
        StringAssert.Contains(error.Message, "'TestPackage' was not found");
    }
}
