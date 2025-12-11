using D20Tek.Tools.DevPomo.Commands.RunTimer;
using Spectre.Console;

namespace D20Tek.Tools.UnitTests.DevPomo.Commands;

[TestClass]
public class PanelDetailsTests
{
    [TestMethod]
    public void Update_WithValues()
    {
        // arrange
        var details = new PanelDetails("", "", Color.Black);

        // act
        var result = details with { Title = "updated", ForegroundColor = "blue", BorderColor = Color.Yellow };

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("updated", result.Title);
        Assert.AreEqual("blue", result.ForegroundColor);
        Assert.AreEqual(Color.Yellow, result.BorderColor);
    }
}
