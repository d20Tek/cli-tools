namespace D20Tek.Tools.JsonMinify;

internal static class Constants
{
    public const string AppName = "json-minify";
    public const string JsonFileExtension = ".json";
    public const string MinifiedJsonFileExtension = ".min.json";
    public const string SingleFileSuccess = "Json file was successfully minified.";

    public static class Errors
    {
        public static Error FilePathRequired = Error.Validation("FilePath.Required", "The file path is required.");

        public static Error FilePathNotFound(string path) =>
            Error.NotFound("FilePath.NotFound", $"The file '{path}' does not exist.");

        public static Error FilePathInvalidExtension(string path) =>
            Error.Validation("FilePath.Extension", $"The file '{path}' is not a json file.");
    }
}
