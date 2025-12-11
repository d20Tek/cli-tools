using D20Tek.Tools.DevPomo.Commands.RunTimer;

namespace D20Tek.Tools.UnitTests.DevPomo.Commands;

[TestClass]
public class TimerPanelTests
{
    [TestMethod]
    public void Create_WithNullBorderColor()
    {
        // arrange

        // act
        var panel = new TimerPanel("test panel");

        // assert
        Assert.IsNotNull(panel);
    }
}
