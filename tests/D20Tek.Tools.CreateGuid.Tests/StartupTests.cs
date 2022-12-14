//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using D20Tek.Spectre.Console.Extensions.Testing;
using D20Tek.Tools.CreateGuid.Services;
using TextCopy;

namespace D20Tek.Tools.CreateGuid.Tests
{
    [TestClass]
    public class StartupTests
    {
        [TestMethod]
        public void ConfigureServices()
        {
            // arrange
            var context = new CommandConfigurationTestContext();
            var startup = new Startup();

            // act
            startup.ConfigureServices(context.Registrar);

            // assert
            Assert.IsInstanceOfType(context.Resolver.Resolve(typeof(IGuidGenerator)), typeof(GuidGenerator));
            Assert.IsInstanceOfType(context.Resolver.Resolve(typeof(IGuidFormatter)), typeof(GuidFormatter));
            Assert.IsInstanceOfType(context.Resolver.Resolve(typeof(IClipboard)), typeof(Clipboard));
        }

        [TestMethod]
        public void ConfigureCommands()
        {
            // arrange
            var context = new CommandConfigurationTestContext();
            var startup = new Startup();

            // act
            var result = startup.ConfigureCommands(context.Configurator);

            // assert
            Assert.AreEqual(context.Configurator, result);
            Assert.AreEqual("create-guid", context.Configurator.Settings.ApplicationName);
            Assert.AreEqual(1, context.Configurator.Commands.Count());
            Assert.AreEqual("generate", context.Configurator.Commands.First().Name);
        }
    }
}
