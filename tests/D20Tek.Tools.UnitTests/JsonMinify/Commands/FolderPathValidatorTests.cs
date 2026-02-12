using D20Tek.Tools.JsonMinify;
using D20Tek.Tools.JsonMinify.Commands;
using D20Tek.Tools.UnitTests.JsonMinify.Fakes;

namespace D20Tek.Tools.UnitTests.JsonMinify.Commands;

[TestClass]
public class FolderPathValidatorTests
{
    [TestMethod]
    public void Validate_ShouldReturnFailure_WhenFolderPathIsEmpty()
    {
        // arrange
        var fileAdapter = new FakeFileSystemAdapter(string.Empty);
        var validator = new FolderPathValidator(fileAdapter);

        // act
        var result = validator.Validate(string.Empty);

        // assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(Constants.Errors.FolderPathRequired, result.GetErrors().First());
    }

    [TestMethod]
    public void Validate_ShouldReturnFailure_WhenFolderDoesNotExist()
    {
        // arrange
        var fileAdapter = new FakeFileSystemAdapter(string.Empty);
        var validator = new FolderPathValidator(fileAdapter);
        var folderPath = "./nonexistent/path";

        // act
        var result = validator.Validate(folderPath);

        // assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(Constants.Errors.FolderPathNotFound(folderPath), result.GetErrors().First());
    }
}
