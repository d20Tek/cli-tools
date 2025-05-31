using D20Tek.Tools.CreateGuid.Services;

namespace D20Tek.Tools.UnitTests.CreateGuid;

[TestClass]
public class GuidGeneratorTests
{
    [TestMethod]
    public void GenerateGuids_WithCountOne_ReturnsOneGuid()
    {
        // arrange
        var gen = new GuidGenerator();

        // act
        var result = gen.GenerateGuids(1, false);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count());
        Assert.IsFalse(result.Contains(Guid.Empty));
    }

    [TestMethod]
    public void GenerateGuids_WithCountOneAndEmpty_ReturnsOneEmptyGuid()
    {
        // arrange
        var gen = new GuidGenerator();

        // act
        var result = gen.GenerateGuids(1, true);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count());
        Assert.IsTrue(result.Contains(Guid.Empty));
    }

    [TestMethod]
    public void GenerateGuids_WithCountFive_ReturnsFiveGuids()
    {
        // arrange
        var gen = new GuidGenerator();

        // act
        var result = gen.GenerateGuids(5, false);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(5, result.Count());
        Assert.IsFalse(result.Contains(Guid.Empty));
    }
}
