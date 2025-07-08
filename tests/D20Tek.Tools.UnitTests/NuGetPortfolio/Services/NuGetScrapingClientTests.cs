using D20Tek.NuGet.Portfolio.Services;
using D20Tek.Tools.UnitTests.NuGetPortfolio.Fakes;

namespace D20Tek.Tools.UnitTests.NuGetPortfolio.Services;

[TestClass]
public class NuGetScrapingClientTests
{
    [TestMethod]
    public async Task GetTotalDownloadsAsync_WithPackageId_ReturnsTotal()
    {
        // arrange
        string testPackageId = "TestPackage";
        string htmlResponse = """
        <html>
          <body>
            <div id="version-history">
              <table>
                <tbody>
                  <tr>
                    <td>1.1.0</td>
                    <td>300</td>
                  </tr>
                  <tr>
                    <td>1.0.2</td>
                    <td>200</td>
                  </tr>
                  <tr>
                    <td>1.0.1</td>
                    <td>100</td>
                  </tr>
                  <tr>
                    <td>1.0.0</td>
                    <td>300</td>
                  </tr>
                </tbody>
              </table>
            </div>
          </body>
        </html>
        """;

        var handler = new FakeHttpMessageHandler(htmlResponse);
        var http = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://www.nuget.org/")
        };
        var client = new NuGetScrapingClient(http);

        // act
        var result = await client.GetTotalDownloadsAsync(testPackageId);

        // assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(900, result.GetValue());
    }

    [TestMethod]
    public async Task GetTotalDownloadsAsync_MissingVersionHistory_ReturnsError()
    {
        // arrange
        string testPackageId = "TestPackage";
        string htmlResponse = """
        <html>
          <body>
            <div id="version-history-bogus">
              <table>
                <tbody>
                  <tr>
                    <td>1.0.0</td>
                    <td>300</td>
                  </tr>
                </tbody>
              </table>
            </div>
          </body>
        </html>
        """;

        var handler = new FakeHttpMessageHandler(htmlResponse);
        var http = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://www.nuget.org/")
        };
        var client = new NuGetScrapingClient(http);

        // act
        var result = await client.GetTotalDownloadsAsync(testPackageId);

        // assert
        Assert.IsTrue(result.IsFailure);
        StringAssert.Contains(result.GetErrors().First().Message, "Could not find the expected version-history");
    }

    [TestMethod]
    public async Task GetTotalDownloadsAsync_InvalidDownloadFormat_ReturnsError()
    {
        // arrange
        string testPackageId = "TestPackage";
        string htmlResponse = """
        <html>
          <body>
            <div id="version-history">
              <table>
                <tbody>
                  <tr>
                    <td>1.0.0</td>
                    <td>error</td>
                  </tr>
                </tbody>
              </table>
            </div>
          </body>
        </html>
        """;

        var handler = new FakeHttpMessageHandler(htmlResponse);
        var http = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://www.nuget.org/")
        };
        var client = new NuGetScrapingClient(http);

        // act
        var result = await client.GetTotalDownloadsAsync(testPackageId);

        // assert
        Assert.IsTrue(result.IsFailure);
        StringAssert.Contains(result.GetErrors().First().Message, "not in a correct format");
    }

    [TestMethod]
    public async Task GetTotalDownloadsAsync_NoDownloadData_ReturnsError()
    {
        // arrange
        string testPackageId = "TestPackage";
        string htmlResponse = """
        <html>
          <body>
            <div id="version-history">
              <table>
                <tbody>
                  <tr>
                    <td>1.0.1</td>
                    <td>0</td>
                  </tr>
                  <tr>
                    <td>1.0.0</td>
                  </tr>
                </tbody>
              </table>
            </div>
          </body>
        </html>
        """;

        var handler = new FakeHttpMessageHandler(htmlResponse);
        var http = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://www.nuget.org/")
        };
        var client = new NuGetScrapingClient(http);

        // act
        var result = await client.GetTotalDownloadsAsync(testPackageId);

        // assert
        Assert.IsTrue(result.IsFailure);
        StringAssert.Contains(result.GetErrors().First().Message, "No downloads were found");
    }

    [TestMethod]
    public async Task GetTotalDownloadsAsync_NoMissingCells_ReturnsError()
    {
        // arrange
        string testPackageId = "TestPackage";
        string htmlResponse = """
        <html>
          <body>
            <div id="version-history">
              <table>
                <tbody>
                  <tr>
                  </tr>
                </tbody>
              </table>
            </div>
          </body>
        </html>
        """;

        var handler = new FakeHttpMessageHandler(htmlResponse);
        var http = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://www.nuget.org/")
        };
        var client = new NuGetScrapingClient(http);

        // act
        var result = await client.GetTotalDownloadsAsync(testPackageId);

        // assert
        Assert.IsTrue(result.IsFailure);
        StringAssert.Contains(result.GetErrors().First().Message, "No downloads were found");
    }
}
