using D20Tek.Spectre.Console.Extensions.Testing;

namespace D20Tek.Tools.UnitTests.CreateGuid;

internal static class CreateGuidCommandValidator
{
    public static void AssertValidWithGuid(this CommandAppResult result, string guidString)
    {
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.ExitCode);
        StringAssert.Contains(result.Output, guidString);
        StringAssert.StartsWith(result.Output, "create-guid: running");
        StringAssert.Contains(result.Output, "Command completed successfully!");
    }

    public static void AssertValidWithThreeGuids(this CommandAppResult result, Guid[] guids)
    {
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.ExitCode);
        StringAssert.Contains(result.Output, guids[0].ToString());
        StringAssert.Contains(result.Output, guids[1].ToString());
        StringAssert.Contains(result.Output, guids[2].ToString());
        Assert.IsFalse(result.Output.Contains(guids[3].ToString()));
        Assert.IsFalse(result.Output.Contains(guids[4].ToString()));
        StringAssert.StartsWith(result.Output, "create-guid: running");
        StringAssert.Contains(result.Output, "Command completed successfully!");
    }
}
