using Spectre.Console.Cli;
using System.Diagnostics.CodeAnalysis;

namespace D20Tek.Tools.UnitTests.NuGetPortfolio.Fakes;

[ExcludeFromCodeCoverage]
internal class FakeCommandApp : ICommandApp
{
    private readonly int _expectedResult;

    public FakeCommandApp(int expectedResult = 0)
    {
        _expectedResult = expectedResult;
    }

    public void Configure(Action<IConfigurator> configuration) => throw new NotImplementedException();

    public int Run(IEnumerable<string> args) => _expectedResult;

    public Task<int> RunAsync(IEnumerable<string> args) => Task.FromResult(_expectedResult);
}
