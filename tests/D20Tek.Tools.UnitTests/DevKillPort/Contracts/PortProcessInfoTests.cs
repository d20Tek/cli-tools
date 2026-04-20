using D20Tek.Tools.DevKillPort.Contracts;

namespace D20Tek.Tools.UnitTests.DevKillPort.Contracts;

[TestClass]
public class PortProcessInfoTests
{
    [TestMethod]
    public void Constructor_SetsProperties()
    {
        // arrange / act
        var info = new PortProcessInfo(5000, 1234, "dotnet", "TCP", "0.0.0.0", PortState.Listen);

        // assert
        Assert.AreEqual(5000, info.Port);
        Assert.AreEqual(1234, info.ProcessId);
        Assert.AreEqual("dotnet", info.ProcessName);
        Assert.AreEqual("TCP", info.Protocol);
        Assert.AreEqual("0.0.0.0", info.Address);
        Assert.AreEqual(PortState.Listen, info.State);
    }

    [TestMethod]
    public void With_ChangesAllProperties_ReturnsNewInstance()
    {
        // arrange
        var info = new PortProcessInfo(5000, 1234, "dotnet", "TCP", "0.0.0.0", PortState.Listen);

        // act
        var result = info with
        {
            Port = 8080,
            ProcessId = 5678,
            ProcessName = "node",
            Protocol = "UDP",
            Address = "127.0.0.1",
            State = PortState.Established
        };

        // assert
        Assert.AreNotSame(info, result);
        Assert.AreEqual(8080, result.Port);
        Assert.AreEqual(5678, result.ProcessId);
        Assert.AreEqual("node", result.ProcessName);
        Assert.AreEqual("UDP", result.Protocol);
        Assert.AreEqual("127.0.0.1", result.Address);
        Assert.AreEqual(PortState.Established, result.State);
    }

    [TestMethod]
    public void Equality_WithSameValues_AreEqual()
    {
        // arrange
        var info1 = new PortProcessInfo(5000, 1234, "dotnet", "TCP", "0.0.0.0", PortState.Listen);
        var info2 = new PortProcessInfo(5000, 1234, "dotnet", "TCP", "0.0.0.0", PortState.Listen);

        // act / assert
        Assert.AreEqual(info1, info2);
    }

    [TestMethod]
    public void Equality_WithDifferentValues_AreNotEqual()
    {
        // arrange
        var info1 = new PortProcessInfo(5000, 1234, "dotnet", "TCP", "0.0.0.0", PortState.Listen);
        var info2 = new PortProcessInfo(8080, 5678, "node", "UDP", "127.0.0.1", PortState.Established);

        // act / assert
        Assert.AreNotEqual(info1, info2);
    }
}
