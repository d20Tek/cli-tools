using D20Tek.Spectre.Console.Extensions.Services;
using D20Tek.Tools.CreateGuid.Services;
using Spectre.Console.Cli;
using TextCopy;

namespace D20Tek.Tools.UnitTests.Fakes;

internal static class RegistrarConfigurator
{
    public static ITypeRegistrar ConfigureServices(this ITypeRegistrar registrar, Guid? guid = null)
    {
        IGuidGenerator guidGen = guid is not null ? new FakeGuidGenerator(guid.Value) : new GuidGenerator();
        registrar.RegisterInstance(typeof(IGuidGenerator), guidGen);
        registrar.Register(typeof(IGuidFormatter), typeof(GuidFormatter));
        registrar.Register(typeof(IClipboard), typeof(FakeClipboard));
        registrar.Register(typeof(IVerbosityWriter), typeof(ConsoleVerbosityWriter));

        return registrar;
    }
}
