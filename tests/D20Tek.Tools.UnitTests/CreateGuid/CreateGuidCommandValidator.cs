using D20Tek.Spectre.Console.Extensions.Testing;

namespace D20Tek.Tools.UnitTests.CreateGuid;

internal static class CreateGuidCommandValidator
{
    public static void AssertValidWithGuid(this CommandAppResult result, string expected)
    {
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.ExitCode);

        StringAssert.Contains(result.Output, expected);
        StringAssert.StartsWith(result.Output, "create-guid: running");
        StringAssert.Contains(result.Output, "Command completed successfully!");
    }

    public static void AssertValidWithThreeGuids(this CommandAppResult result, Guid[] expected)
    {
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.ExitCode);

        StringAssert.Contains(result.Output, expected[0].ToString());
        StringAssert.Contains(result.Output, expected[1].ToString());
        StringAssert.Contains(result.Output, expected[2].ToString());
        Assert.IsFalse(result.Output.Contains(expected[3].ToString()));
        Assert.IsFalse(result.Output.Contains(expected[4].ToString()));

        StringAssert.StartsWith(result.Output, "create-guid: running");
        StringAssert.Contains(result.Output, "Command completed successfully!");
    }
}
