//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using D20Tek.Tools.CreateGuid.Services;

namespace D20Tek.Tools.CreateGuid.Tests.Services
{
    [TestClass]
    public class GuidFormatterTests
    {
        [TestMethod]
        public void FormatGuidDefault()
        {
            // arrange
            var service = new GuidFormatter();
            var input = "1d8d01cf-430e-43e1-a151-807f05e493b9";
            var guid = new Guid(input);

            // act
            var actual = service.Format(guid, GuidFormat.Default, false);

            // assert
            Assert.AreEqual(input, actual);
        }

        [TestMethod]
        public void FormatGuidBraces()
        {
            // arrange
            var service = new GuidFormatter();
            var input = "{1d8d01cf-430e-43e1-a151-807f05e493b9}";
            var guid = new Guid(input);

            // act
            var actual = service.Format(guid, GuidFormat.Braces, false);

            // assert
            Assert.AreEqual(input, actual);
        }

        [TestMethod]
        public void FormatGuidUpperCase()
        {
            // arrange
            var service = new GuidFormatter();
            var input = "{1d8d01cf-430e-43e1-a151-807f05e493b9}";
            var guid = new Guid(input);

            // act
            var actual = service.Format(guid, GuidFormat.Braces, true);

            // assert
            Assert.AreEqual(input.ToUpper(), actual);
        }
    }
}
