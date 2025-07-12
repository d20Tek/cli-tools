using D20Tek.NuGet.Portfolio;
using D20Tek.NuGet.Portfolio.Abstractions;
using D20Tek.NuGet.Portfolio.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace D20Tek.Tools.UnitTests.NuGetPortfolio;

[TestClass]
public class AppServiceCollectionTests
{

    [TestMethod]
    public void Initialize_RegistersServices()
    {
        // arrange

        // act
        var services = AppServiceCollection.Initialize();
        var provider = services.BuildServiceProvider();

        // assert
        Assert.IsNotNull(services.FirstOrDefault(s => s.ServiceType == typeof(AppDbContext)));
        Assert.IsNotNull(services.FirstOrDefault(s => s.ServiceType == typeof(ILoggerFactory)));
        Assert.IsNotNull(services.FirstOrDefault(s => s.ServiceType == typeof(INuGetSearchClient)));
        Assert.IsNotNull(provider.GetService<INuGetSearchClient>());
    }
}
