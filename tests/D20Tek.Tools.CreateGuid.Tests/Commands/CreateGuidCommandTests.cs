//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using D20Tek.Spectre.Console.Extensions.Services;
using D20Tek.Spectre.Console.Extensions.Settings;
using D20Tek.Spectre.Console.Extensions.Testing;
using D20Tek.Tools.CreateGuid.Commands;
using D20Tek.Tools.CreateGuid.Services;
using D20Tek.Tools.CreateGuid.Tests.Mocks;
using TextCopy;

namespace D20Tek.Tools.CreateGuid.Tests.Commands
{
    [TestClass]
    public class CreateGuidCommandTests
    {
        private const string _defaultGuidText = "1d8d01cf-430e-43e1-a151-807f05e493b9";
        private static readonly Guid _defaultTestGuid = new Guid(_defaultGuidText);
        private static readonly IGuidFormatter _guidFormatter = new GuidFormatter();
        private readonly TestConsole _console = new TestConsole();
        private readonly IVerbosityWriter _displayWriter;
        private readonly Mock<IClipboard> _clipboard = new Mock<IClipboard>();

        public CreateGuidCommandTests()
        {
            _displayWriter = new ConsoleVerbosityWriter(_console);

            _clipboard.Setup(p => p.SetText(It.IsAny<string>())).Verifiable();
        }

        [TestMethod]
        public void Execute_WithDefaultSettings()
        {
            // arrange
            var context = MockCommandContext.Get();
            var command = new CreateGuidCommand(
                InitializeMockGenerator().Object, _guidFormatter, _displayWriter, _clipboard.Object);
            var settings = new GuidSettings();

            // act
            var result = command.Execute(context, settings);

            // assert
            Assert.AreEqual(0, result);
            Assert.IsTrue(_console.Output.Contains(_defaultGuidText));
            Assert.IsTrue(_console.Output.Contains("create-guid"));
            Assert.IsTrue(_console.Output.Contains("successfully"));
            _clipboard.Verify(o => o.SetText(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void Execute_WithEmptyGuids()
        {
            // arrange
            var context = MockCommandContext.Get();
            var command = new CreateGuidCommand(
                InitializeMockGenerator(Guid.Empty).Object, _guidFormatter, _displayWriter, _clipboard.Object);
            var settings = new GuidSettings { Count = 5, Format = GuidFormat.Default, UsesEmptyGuid = true };

            // act
            var result = command.Execute(context, settings);

            // assert
            Assert.AreEqual(0, result);
            Assert.IsTrue(_console.Output.Contains(Guid.Empty.ToString()));
            Assert.IsTrue(_console.Output.Contains("create-guid"));
            Assert.IsTrue(_console.Output.Contains("successfully"));
            _clipboard.Verify(o => o.SetText(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void Execute_WithUpperCase()
        {
            // arrange
            var context = MockCommandContext.Get();
            var command = new CreateGuidCommand(
                InitializeMockGenerator().Object, _guidFormatter, _displayWriter, _clipboard.Object);
            var settings = new GuidSettings { UsesUpperCase = true };

            // act
            var result = command.Execute(context, settings);

            // assert
            Assert.AreEqual(0, result);
            Assert.IsTrue(_console.Output.Contains(_defaultGuidText.ToUpper()));
            Assert.IsTrue(_console.Output.Contains("create-guid"));
            Assert.IsTrue(_console.Output.Contains("successfully"));
            _clipboard.Verify(o => o.SetText(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void Execute_WithVerbosityMinimal()
        {
            // arrange
            var context = MockCommandContext.Get();
            var command = new CreateGuidCommand(
                InitializeMockGenerator().Object, _guidFormatter, _displayWriter, _clipboard.Object);
            var settings = new GuidSettings { Verbosity = VerbosityLevel.Minimal };

            // act
            var result = command.Execute(context, settings);

            // assert
            Assert.AreEqual(0, result);
            Assert.IsTrue(_console.Output.Contains(_defaultGuidText));
            Assert.IsFalse(_console.Output.Contains("create-guid"));
            Assert.IsFalse(_console.Output.Contains("successfully"));
            _clipboard.Verify(o => o.SetText(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void Execute_WithCopyToClipboard()
        {
            // arrange
            var context = MockCommandContext.Get();
            var command = new CreateGuidCommand(
                InitializeMockGenerator().Object, _guidFormatter, _displayWriter, _clipboard.Object);
            var settings = new GuidSettings { CopyToClipboard = true };

            // act
            var result = command.Execute(context, settings);

            // assert
            Assert.AreEqual(0, result);
            Assert.IsTrue(_console.Output.Contains(_defaultGuidText));
            Assert.IsTrue(_console.Output.Contains("create-guid"));
            Assert.IsTrue(_console.Output.Contains("successfully"));
            _clipboard.Verify(o => o.SetText(It.IsAny<string>()), Times.Once);
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
