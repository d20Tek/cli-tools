using D20Tek.Tools.DevPomo.Commands.RunTimer;

namespace D20Tek.Tools.UnitTests.DevPomo.Commands;

[TestClass]
public class TimerInputHandlerTests
{
    [TestMethod]
    public void Dispose_WithNullTimer_SkipsStop()
    {
        // arrange
        var console = new TestConsole();
        var state = new TimerState();

        var handler = new TimerInputHandler(console, state);

        // act
        handler.Dispose();

        // assert
    }
}
