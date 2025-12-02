namespace D20Tek.Tools.UnitTests.NuGetPortfolio.Fakes;

[ExcludeFromCodeCoverage]
internal class FakeCommandApp(int expectedResult = 0) : ICommandApp
{
    private readonly int _expectedResult = expectedResult;

    public void Configure(Action<IConfigurator> configuration) => throw new NotImplementedException();

    public int Run(IEnumerable<string> args, CancellationToken token) => _expectedResult;

    public Task<int> RunAsync(IEnumerable<string> args, CancellationToken token) => Task.FromResult(_expectedResult);
}
