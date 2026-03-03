using D20Tek.Tools.DevLog.Contracts;

namespace D20Tek.Tools.UnitTests.DevLog.Contracts;

[TestClass]
public class DevLogEntryTests
{
    private static readonly DateOnly _weekStart = new(2025, 1, 5);
    private static readonly List<string> _accomplishments = ["Did thing 1", "Did thing 2"];

    [TestMethod]
    public void Constructor_WithValidData_SetsProperties()
    {
        // arrange / act
        var entry = new DevLogEntry(_weekStart, "MyProject", _accomplishments);

        // assert
        Assert.AreEqual(_weekStart, entry.WeekStart);
        Assert.AreEqual("MyProject", entry.ProjectName);
        Assert.AreEqual(_accomplishments, entry.Accomplishments);
    }

    [TestMethod]
    public void With_ChangesAllProperties_ReturnsNewInstance()
    {
        // arrange
        var entry = new DevLogEntry(_weekStart, "MyProject", _accomplishments);
        var newWeekStart = new DateOnly(2025, 1, 12);
        var newAccomplishments = new List<string> { "New accomplishment" };

        // act
        var result = entry with
        {
            WeekStart = newWeekStart,
            ProjectName = "OtherProject",
            Accomplishments = newAccomplishments
        };

        // assert
        Assert.AreNotSame(entry, result);
        Assert.AreEqual(newWeekStart, result.WeekStart);
        Assert.AreEqual("OtherProject", result.ProjectName);
        Assert.AreEqual(newAccomplishments, result.Accomplishments);
    }

    [TestMethod]
    public void Equality_WithSameValues_AreEqual()
    {
        // arrange
        var entry1 = new DevLogEntry(_weekStart, "MyProject", _accomplishments);
        var entry2 = new DevLogEntry(_weekStart, "MyProject", _accomplishments);

        // act / assert
        Assert.AreEqual(entry1, entry2);
    }

    [TestMethod]
    public void Equality_WithDifferentValues_AreNotEqual()
    {
        // arrange
        var entry1 = new DevLogEntry(_weekStart, "MyProject", _accomplishments);
        var entry2 = new DevLogEntry(_weekStart, "OtherProject", _accomplishments);

        // act / assert
        Assert.AreNotEqual(entry1, entry2);
    }

    [TestMethod]
    public void Create_WithDateParam_SetsCorrectWeekStart()
    {
        // arrange
        var wednesday = new DateOnly(2025, 1, 8);
        var expectedWeekStart = new DateOnly(2025, 1, 5);

        // act
        var entry = DevLogEntry.Create("MyProject", _accomplishments, wednesday);

        // assert
        Assert.AreEqual(expectedWeekStart, entry.WeekStart);
        Assert.AreEqual("MyProject", entry.ProjectName);
        Assert.AreEqual(_accomplishments, entry.Accomplishments);
    }

    [TestMethod]
    public void Create_WithNoDate_UsesTodaysWeekStart()
    {
        // arrange
        var expectedWeekStart = DevLogEntry.GetWeekStart(DateOnly.FromDateTime(DateTime.Today));

        // act
        var entry = DevLogEntry.Create("MyProject", _accomplishments);

        // assert
        Assert.AreEqual(expectedWeekStart, entry.WeekStart);
    }

    [TestMethod]
    public void GetWeekStart_WithSunday_ReturnsSameDay()
    {
        // arrange
        var sunday = new DateOnly(2025, 1, 5);

        // act
        var result = DevLogEntry.GetWeekStart(sunday);

        // assert
        Assert.AreEqual(sunday, result);
    }

    [TestMethod]
    public void GetWeekStart_WithMidWeekDate_ReturnsPreviousSunday()
    {
        // arrange
        var wednesday = new DateOnly(2025, 1, 8);
        var expectedSunday = new DateOnly(2025, 1, 5);

        // act
        var result = DevLogEntry.GetWeekStart(wednesday);

        // assert
        Assert.AreEqual(expectedSunday, result);
    }

    [TestMethod]
    public void GetWeekStart_WithSaturday_ReturnsPreviousSunday()
    {
        // arrange
        var saturday = new DateOnly(2025, 1, 11);
        var expectedSunday = new DateOnly(2025, 1, 5);

        // act
        var result = DevLogEntry.GetWeekStart(saturday);

        // assert
        Assert.AreEqual(expectedSunday, result);
    }
}
