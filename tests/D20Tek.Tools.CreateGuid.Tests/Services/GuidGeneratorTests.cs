//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using D20Tek.Tools.CreateGuid.Services;

namespace D20Tek.Tools.CreateGuid.Tests.Services
{
    [TestClass]
    public class GuidGeneratorTests
    {
        [TestMethod]
        public void GenerateSingleGuid()
        {
            // arrange
            var service = new GuidGenerator();

            // act
            var results = service.GenerateGuids(1, false);

            // assert
            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count());
            Assert.AreNotEqual(Guid.Empty, results.First());
        }

        [TestMethod]
        public void GenerateMultipleGuids()
        {
            // arrange
            var service = new GuidGenerator();

            // act
            var results = service.GenerateGuids(5, false);

            // assert
            Assert.IsNotNull(results);
            Assert.AreEqual(5, results.Count());
            Assert.AreEqual(0, results.Where(x => x.Equals(Guid.Empty)).Count());
        }

        [TestMethod]
        public void GenerateEmptyGuids()
        {
            // arrange
            var service = new GuidGenerator();

            // act
            var results = service.GenerateGuids(3, true);

            // assert
            Assert.IsNotNull(results);
            Assert.AreEqual(3, results.Count());
            Assert.AreEqual(3, results.Where(x => x.Equals(Guid.Empty)).Count());
        }
    }
}
