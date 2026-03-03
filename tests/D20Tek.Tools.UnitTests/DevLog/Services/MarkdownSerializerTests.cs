using D20Tek.Tools.DevLog.Contracts;
using D20Tek.Tools.DevLog.Services;

namespace D20Tek.Tools.UnitTests.DevLog.Services;

[TestClass]
public class MarkdownSerializerTests
{
    private static readonly DateOnly _weekStart = new(2025, 1, 5);

    [TestMethod]
    public void ParseEntries_WithSingleProject_ReturnsEntry()
    {
        // arrange
        var content = "## Week of January 5, 2025\n\n### MyProject\n- Did thing 1\n- Did thing 2";

        // act
        var entries = MarkdownSerializer.ParseEntries(content, _weekStart);

        // assert
        Assert.HasCount(1, entries);
        Assert.AreEqual("MyProject", entries[0].ProjectName);
        Assert.HasCount(2, entries[0].Accomplishments);
        Assert.AreEqual("Did thing 1", entries[0].Accomplishments[0]);
        Assert.AreEqual("Did thing 2", entries[0].Accomplishments[1]);
    }

    [TestMethod]
    public void ParseEntries_WithMultipleProjects_ReturnsAllEntries()
    {
        // arrange
        var content = "## Week of January 5, 2025\n\n### Project1\n- Item 1\n\n### Project2\n- Item 2";

        // act
        var entries = MarkdownSerializer.ParseEntries(content, _weekStart);

        // assert
        Assert.HasCount(2, entries);
        Assert.AreEqual("Project1", entries[0].ProjectName);
        Assert.AreEqual("Project2", entries[1].ProjectName);
    }

    [TestMethod]
    public void ParseEntries_WithEmptyContent_ReturnsEmptyList()
    {
        // arrange
        var content = string.Empty;

        // act
        var entries = MarkdownSerializer.ParseEntries(content, _weekStart);

        // assert
        Assert.HasCount(0, entries);
    }

    [TestMethod]
    public void ParseEntries_WithNoProjectSections_ReturnsEmptyList()
    {
        // arrange
        var content = "## Week of January 5, 2025\n\nSome random text here.";

        // act
        var entries = MarkdownSerializer.ParseEntries(content, _weekStart);

        // assert
        Assert.HasCount(0, entries);
    }

    [TestMethod]
    public void SerializeEntries_WithSingleEntry_ReturnsFormattedMarkdown()
    {
        // arrange
        var entries = new List<DevLogEntry>
        {
            new(_weekStart, "MyProject", ["Did thing 1", "Did thing 2"])
        };

        // act
        var result = MarkdownSerializer.SerializeEntries(_weekStart, entries);

        // assert
        Assert.Contains("## Week of January 5, 2025", result);
        Assert.Contains("### MyProject", result);
        Assert.Contains("- Did thing 1", result);
        Assert.Contains("- Did thing 2", result);
    }

    [TestMethod]
    public void SerializeEntries_WithMultipleEntries_IncludesAllProjects()
    {
        // arrange
        var entries = new List<DevLogEntry>
        {
            new(_weekStart, "Project1", ["Item 1"]),
            new(_weekStart, "Project2", ["Item 2"])
        };

        // act
        var result = MarkdownSerializer.SerializeEntries(_weekStart, entries);

        // assert
        Assert.Contains("### Project1", result);
        Assert.Contains("### Project2", result);
    }

    [TestMethod]
    public void RoundTrip_ParseThenSerialize_ProducesConsistentOutput()
    {
        // arrange
        var original = "## Week of January 5, 2025\n\n### MyProject\n- Did thing 1\n- Did thing 2";

        // act
        var entries = MarkdownSerializer.ParseEntries(original, _weekStart);
        var serialized = MarkdownSerializer.SerializeEntries(_weekStart, entries);

        // assert
        Assert.AreEqual(original, serialized);
    }
}
