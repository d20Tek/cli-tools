using System.Net;
using System.Text;

namespace D20Tek.Tools.UnitTests.NuGetPortfolio.Fakes;

internal class FakeHttpMessageHandler(string jsonContent) : HttpMessageHandler
{
    private readonly string _jsonContent = jsonContent;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token) =>
        Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(_jsonContent, Encoding.UTF8, "application/json")
        });
}

internal class ErrorHttpMessageHandler : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token) =>
        throw new InvalidOperationException();
}
