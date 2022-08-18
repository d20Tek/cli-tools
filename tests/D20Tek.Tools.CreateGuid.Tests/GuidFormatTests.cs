//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------

namespace D20Tek.Tools.CreateGuid.Tests
{
    [TestClass]
    public class GuidFormatTests
    {
        [DataTestMethod]
        [DataRow(GuidFormat.Default, "D", DisplayName = "ToFormatString-Default")]
        [DataRow(GuidFormat.D, "D", DisplayName = "ToFormatString-D")]
        [DataRow(GuidFormat.Number, "N", DisplayName = "ToFormatString-Number")]
        [DataRow(GuidFormat.N, "N", DisplayName = "ToFormatString-N")]
        [DataRow(GuidFormat.Braces, "B", DisplayName = "ToFormatString-Braces")]
        [DataRow(GuidFormat.B, "B", DisplayName = "ToFormatString-B")]
        [DataRow(GuidFormat.Parens, "P", DisplayName = "ToFormatString-Parens")]
        [DataRow(GuidFormat.P, "P", DisplayName = "ToFormatString-P")]
        [DataRow(GuidFormat.Hex, "X", DisplayName = "ToFormatString-Hex")]
        [DataRow(GuidFormat.X, "X", DisplayName = "ToFormatString-X")]
        public void ToFormatString(GuidFormat format, string expected)
        {
            // arrange

            // act
            var actual = format.ToFormatString();

            // assert
            Assert.AreEqual(expected, actual);
        }
    }
}
