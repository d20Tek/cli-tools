//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using D20Tek.Spectre.Console.Extensions.Services;
using D20Tek.Tools.CreateGuid.Commands;
using D20Tek.Tools.CreateGuid.Services;
using D20Tek.Tools.CreateGuid.Tests.Mocks;
using TextCopy;

namespace D20Tek.Tools.CreateGuid.Tests.Commands
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class CreateCommandErrorTests
    {
        private static readonly IGuidFormatter _testFormatter = new Mock<IGuidFormatter>().Object;
        private static readonly IGuidGenerator _testGenerator = new Mock<IGuidGenerator>().Object;
        private static readonly IVerbosityWriter _displayWriter = new Mock<IVerbosityWriter>().Object;
        private static readonly IClipboard _clipboard = new Mock<IClipboard>().Object;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_WithNullGenerator()
        {
            // arrange

            // act - assert
            _ = new CreateGuidCommand(null, _testFormatter, _displayWriter, _clipboard);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_WithNullFormatter()
        {
            // arrange

            // act - assert
            _ = new CreateGuidCommand(_testGenerator, null, _displayWriter, _clipboard);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_WithNullWriter()
        {
            // arrange

            // act - assert
            _ = new CreateGuidCommand(_testGenerator, _testFormatter, null, _clipboard);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_WithNullClipboard()
        {
            // arrange

            // act - assert
            _ = new CreateGuidCommand(_testGenerator, _testFormatter, _displayWriter, null);
        }
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Execute_WithException()
        {
            // arrange
            var context = MockCommandContext.Get();
            var settings = new GuidSettings();
            var mockGen = new Mock<IGuidGenerator>();
            mockGen.Setup(x => x.GenerateGuids(It.IsAny<int>(), It.IsAny<bool>()))
                   .Throws<InvalidOperationException>();

            var command = new CreateGuidCommand(mockGen.Object, _testFormatter, _displayWriter, _clipboard);

            // act - assert
            _ = command.Execute(context, settings);
        }
    }
}
