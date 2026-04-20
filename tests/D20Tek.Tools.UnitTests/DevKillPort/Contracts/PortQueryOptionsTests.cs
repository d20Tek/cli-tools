using D20Tek.Tools.DevKillPort.Contracts;

namespace D20Tek.Tools.UnitTests.DevKillPort.Contracts;

[TestClass]
public class PortQueryOptionsTests
{
    [TestMethod]
    public void Constructor_WithDefaults_SetsProperties()
    {
        // arrange / act
        var options = new PortQueryOptions();

        // assert
        Assert.AreEqual(ProtocolType.Both, options.Protocol);
        Assert.IsFalse(options.Force);
        Assert.IsFalse(options.All);
        Assert.IsFalse(options.DryRun);
        Assert.IsFalse(options.Json);
        Assert.IsFalse(options.Watch);
        Assert.AreEqual(30, options.Timeout);
    }

    [TestMethod]
    public void Constructor_WithCustomValues_SetsProperties()
    {
        // arrange / act
        var options = new PortQueryOptions(ProtocolType.Tcp, true, true, true, true, true, 60);

        // assert
        Assert.AreEqual(ProtocolType.Tcp, options.Protocol);
        Assert.IsTrue(options.Force);
        Assert.IsTrue(options.All);
        Assert.IsTrue(options.DryRun);
        Assert.IsTrue(options.Json);
        Assert.IsTrue(options.Watch);
        Assert.AreEqual(60, options.Timeout);
    }

    [TestMethod]
    public void With_ChangesAllProperties_ReturnsNewInstance()
    {
        // arrange
        var options = new PortQueryOptions();

        // act
        var result = options with
        {
            Protocol = ProtocolType.Udp,
            Force = true,
            All = true,
            DryRun = true,
            Json = true,
            Watch = true,
            Timeout = 120
        };

        // assert
        Assert.AreNotSame(options, result);
        Assert.AreEqual(ProtocolType.Udp, result.Protocol);
        Assert.IsTrue(result.Force);
        Assert.IsTrue(result.All);
        Assert.IsTrue(result.DryRun);
        Assert.IsTrue(result.Json);
        Assert.IsTrue(result.Watch);
        Assert.AreEqual(120, result.Timeout);
    }

    [TestMethod]
    public void Equality_WithSameValues_AreEqual()
    {
        // arrange
        var options1 = new PortQueryOptions(ProtocolType.Tcp, true, false, true, false, false, 30);
        var options2 = new PortQueryOptions(ProtocolType.Tcp, true, false, true, false, false, 30);

        // act / assert
        Assert.AreEqual(options1, options2);
    }

    [TestMethod]
    public void Equality_WithDifferentValues_AreNotEqual()
    {
        // arrange
        var options1 = new PortQueryOptions(ProtocolType.Tcp);
        var options2 = new PortQueryOptions(ProtocolType.Udp);

        // act / assert
        Assert.AreNotEqual(options1, options2);
    }
}
