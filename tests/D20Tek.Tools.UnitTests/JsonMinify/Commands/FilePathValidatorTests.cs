using D20Tek.Tools.JsonMinify;
using D20Tek.Tools.JsonMinify.Commands;
using D20Tek.Tools.UnitTests.JsonMinify.Fakes;

namespace D20Tek.Tools.UnitTests.JsonMinify.Commands;

[TestClass]
public class FilePathValidatorTests
{
    [TestMethod]
    public void Validate_ShouldReturnFailure_WhenFilePathIsEmpty()
    {
        // arrange
        var fileAdapter = new FakeFileSystemAdapter(string.Empty);
        var validator = new FilePathValidator(fileAdapter);

        // act
        var result = validator.Validate(string.Empty);

        // assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(Constants.Errors.FilePathRequired, result.GetErrors().First());
    }

    [TestMethod]
    public void Validate_ShouldReturnFailure_WhenFileDoesNotExist()
    {
        // arrange
        var fileAdapter = new FakeFileSystemAdapter(string.Empty);
        var validator = new FilePathValidator(fileAdapter);
        var filePath = "./nonexistent.json";

        // act
        var result = validator.Validate(filePath);

        // assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(Constants.Errors.FilePathNotFound(filePath), result.GetErrors().First());
    }

    [TestMethod]
    public void Validate_ShouldReturnFailure_WhenFileHasInvalidExtension()
    {
        // arrange
        var fileAdapter = new FakeFileSystemAdapter("{}");
        var validator = new FilePathValidator(fileAdapter);
        var filePath = "./file.txt";

        // act
        var result = validator.Validate(filePath);

        // assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(Constants.Errors.FilePathInvalidExtension(filePath), result.GetErrors().First());
    }
}
