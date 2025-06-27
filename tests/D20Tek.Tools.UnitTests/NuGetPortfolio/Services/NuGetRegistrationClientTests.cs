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
            "items": [
                {
                    "items": [
                        { "catalogEntry": { "downloads": 100 } },
                        { "catalogEntry": { "downloads": 200 } }
                    ]
                },
                {
                    "items": [
                        { "catalogEntry": { "downloads": 300 } }
                    ]
                }
            ]
        }
        """;

        var handler = new FakeHttpMessageHandler(jsonResponse);
        var http = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.nuget.org/v3/registration5-gz-semver2/")
        };
        var cache = new MemoryCache(new MemoryCacheOptions());
        var client = new NuGetRegistrationClient(http, cache);

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
            BaseAddress = new Uri("https://api.nuget.org/v3/registration5-gz-semver2/")
        };
        var cache = new MemoryCache(new MemoryCacheOptions());
        var client = new NuGetRegistrationClient(http, cache);

        // act
        var result = await client.GetTotalDownloadsAsync(testPackageId);

        // assert
        Assert.IsTrue(result.IsFailure);
        var error = result.GetErrors().FirstOrDefault();
        Assert.AreEqual("General.Exception", error.Code);
    }
}
