using D20Tek.Spectre.Console.Extensions.Testing;

namespace D20Tek.Tools.UnitTests.CreateGuid;

internal static class CreateGuidCommandValidator
{
    public static void AssertValidWithGuid(this CommandAppResult result, string expected)
    {
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.ExitCode);

        Assert.Contains(expected, result.Output);
        Assert.StartsWith("create-guid: running", result.Output);
        Assert.Contains("Command completed successfully!", result.Output);
    }

    public static void AssertValidWithThreeGuids(this CommandAppResult result, Guid[] expected)
    {
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.ExitCode);

        Assert.Contains(expected[0].ToString(), result.Output);
        Assert.Contains(expected[1].ToString(), result.Output);
        Assert.Contains(expected[2].ToString(), result.Output);
        Assert.DoesNotContain(expected[3].ToString(), result.Output);
        Assert.DoesNotContain(expected[4].ToString(), result.Output);

        Assert.StartsWith("create-guid: running", result.Output);
        Assert.Contains("Command completed successfully!", result.Output);
    }
}
