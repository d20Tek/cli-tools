//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using D20Tek.Tools.CreateGuid.Commands;
using D20Tek.Tools.CreateGuid.Services;
using D20Tek.Tools.CreateGuid.Tests.Common;
using Spectre.Console;

namespace D20Tek.Tools.CreateGuid.Tests.Commands
{
    [TestClass]
    public class CreateGuidCommandTests
    {
        private const string _defaultGuidText = "1d8d01cf-430e-43e1-a151-807f05e493b9";
        private static readonly Guid _defaultTestGuid = new Guid(_defaultGuidText);
        private static readonly IGuidFormatter _guidFormatter = new GuidFormatter();

        [TestMethod]
        public void Execute_WithDefaultSettings()
        {
            // arrange
            var context = new CommandTestContext();
            var command = new CreateGuidCommand(InitializeMockGenerator().Object, _guidFormatter);
            var settings = new GuidSettings();

            // act
            AnsiConsole.Record();
            var result = command.Execute(context.CommandContext, settings);

            // assert
            Assert.AreEqual(0, result);
            Assert.IsTrue(AnsiConsole.ExportText().Contains(_defaultGuidText));
        }

        [TestMethod]
        public void Execute_WithEmptyGuids()
        {
            // arrange
            var context = new CommandTestContext();
            var command = new CreateGuidCommand(InitializeMockGenerator(Guid.Empty).Object, _guidFormatter);
            var settings = new GuidSettings { Count = 5, Format = GuidFormat.Default, UsesEmptyGuid = true };

            // act
            AnsiConsole.Record();
            var result = command.Execute(context.CommandContext, settings);

            // assert
            Assert.AreEqual(0, result);
            Assert.IsTrue(AnsiConsole.ExportText().Contains(Guid.Empty.ToString()));
        }

        [TestMethod]
        public void Execute_WithUpperCase()
        {
            // arrange
            var context = new CommandTestContext();
            var command = new CreateGuidCommand(InitializeMockGenerator().Object, _guidFormatter);
            var settings = new GuidSettings { UsesUpperCase = true };

            // act
            AnsiConsole.Record();
            var result = command.Execute(context.CommandContext, settings);

            // assert
            Assert.AreEqual(0, result);
            Assert.IsTrue(AnsiConsole.ExportText().Contains(_defaultGuidText.ToUpper()));
        }

        private Mock<IGuidGenerator> InitializeMockGenerator(Guid? expectedGuid = null)
        {
            var guid = expectedGuid ?? _defaultTestGuid;
            var mockGen = new Mock<IGuidGenerator>();
            mockGen.Setup(x => x.GenerateGuids(It.IsAny<int>(), It.IsAny<bool>()))
                   .Returns(new Guid[] { guid });

            return mockGen;
        }
    }
}
