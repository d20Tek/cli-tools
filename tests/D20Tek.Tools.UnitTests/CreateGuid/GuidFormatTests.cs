using D20Tek.Tools.CreateGuid.Contracts;
using D20Tek.Tools.CreateGuid.Services;

namespace D20Tek.Tools.UnitTests.CreateGuid;

[TestClass]
public sealed class GuidFormatTests
{
    [TestMethod]
    [DataRow(GuidFormat.D, "D")]
    [DataRow(GuidFormat.Default, "D")]
    [DataRow(GuidFormat.N, "N")]
    [DataRow(GuidFormat.Number, "N")]
    [DataRow(GuidFormat.B, "B")]
    [DataRow(GuidFormat.Braces, "B")]
    [DataRow(GuidFormat.P, "P")]
    [DataRow(GuidFormat.Parens, "P")]
    [DataRow(GuidFormat.X, "X")]
    [DataRow(GuidFormat.Hex, "X")]
    public void ToFormatString_WithGuidFormat_MapsToExpectedString(GuidFormat format, string expected)
    {
        // arrange

        // act
        var result = format.ToFormatString();

        // assert
        Assert.AreEqual(expected, result);
    }
}
