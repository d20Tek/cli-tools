using D20Tek.Spectre.Console.Extensions.Services;
using D20Tek.Tools.CreateGuid.Services;
using D20Tek.Tools.UnitTests.CreateGuid.Fakes;
using Spectre.Console.Cli;
using TextCopy;

namespace D20Tek.Tools.UnitTests.CreateGuid;

internal static class RegistrarConfigurator
{
    public static ITypeRegistrar ConfigureServices(this ITypeRegistrar registrar, Guid[] guids)
    {
        var guidGen = new FakeGuidGenerator(guids);
        registrar.RegisterInstance(typeof(IGuidGenerator), guidGen);
        registrar.Register(typeof(IGuidFormatter), typeof(GuidFormatter));
        registrar.Register(typeof(IClipboard), typeof(FakeClipboard));
        registrar.Register(typeof(IVerbosityWriter), typeof(ConsoleVerbosityWriter));

        return registrar;
    }
}
