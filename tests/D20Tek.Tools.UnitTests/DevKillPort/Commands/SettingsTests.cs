using D20Tek.Tools.DevKillPort.Commands;
using D20Tek.Tools.DevKillPort.Contracts;

namespace D20Tek.Tools.UnitTests.DevKillPort.Commands;

[TestClass]
public class SettingsTests
{
    [TestMethod]
    public void ToQueryOptions_WithDefaults_ReturnsDefaultOptions()
    {
        // arrange
        var settings = new PortSettings { Port = 5000 };

        // act
        var options = settings.ToQueryOptions();

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
    public void ToQueryOptions_WithTcpProtocol_ReturnsTcp()
    {
        // arrange
        var settings = new PortSettings { Port = 5000, Protocol = "tcp" };

        // act
        var options = settings.ToQueryOptions();

        // assert
        Assert.AreEqual(ProtocolType.Tcp, options.Protocol);
    }

    [TestMethod]
    public void ToQueryOptions_WithUdpProtocol_ReturnsUdp()
    {
        // arrange
        var settings = new PortSettings { Port = 5000, Protocol = "udp" };

        // act
        var options = settings.ToQueryOptions();

        // assert
        Assert.AreEqual(ProtocolType.Udp, options.Protocol);
    }

    [TestMethod]
    public void ToQueryOptions_WithAllFlags_SetsAllOptions()
    {
        // arrange
        var settings = new PortSettings
        {
            Port = 5000,
            Force = true,
            All = true,
            DryRun = true,
            Json = true,
            Watch = true,
            Timeout = 60,
            Protocol = "tcp"
        };

        // act
        var options = settings.ToQueryOptions();

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
    public void ToQueryOptions_WithUnknownProtocol_DefaultsToBoth()
    {
        // arrange
        var settings = new PortSettings { Port = 5000, Protocol = "unknown" };

        // act
        var options = settings.ToQueryOptions();

        // assert
        Assert.AreEqual(ProtocolType.Both, options.Protocol);
    }
}
