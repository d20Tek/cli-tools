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
        string testPackageId = "TestPackage2";
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
        Assert.Contains("Could not find the expected version-history", result.GetErrors().First().Message);
    }

    [TestMethod]
    public async Task GetTotalDownloadsAsync_InvalidDownloadFormat_ReturnsError()
    {
        // arrange
        string testPackageId = "TestPackage3";
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
        Assert.Contains("not in a correct format", result.GetErrors().First().Message);
    }

    [TestMethod]
    public async Task GetTotalDownloadsAsync_NoDownloadData_ReturnsError()
    {
        // arrange
        string testPackageId = "TestPackage4";
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
        Assert.Contains("No downloads were found", result.GetErrors().First().Message);
    }

    [TestMethod]
    public async Task GetTotalDownloadsAsync_NoMissingCells_ReturnsError()
    {
        // arrange
        string testPackageId = "TestPackage5";
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
        Assert.Contains("No downloads were found", result.GetErrors().First().Message);
    }
}
