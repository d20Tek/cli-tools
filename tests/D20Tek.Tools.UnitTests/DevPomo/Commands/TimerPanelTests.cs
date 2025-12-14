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
        var panel = new TimerPanel("test panel", false);

        // assert
        Assert.IsNotNull(panel);
    }

    [TestMethod]
    public void Render_WithMinimalOutput_ShowsCompactOutout()
    {
        // arrange
        var panel = new TimerPanel("test panel", true);

        // act
        var result = panel.Render(59, 60, false);

        // assert
        Assert.IsNotNull(result);

    }
}
