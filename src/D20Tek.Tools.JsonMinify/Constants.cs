namespace D20Tek.Tools.JsonMinify;

internal static class Constants
{
    public const string AppName = "json-minify";
    public const string JsonFileExtension = ".json";
    public const string MinifiedJsonFileExtension = ".min.json";
    public const string JsonFileSearchPattern = "*.json";
    public const string SingleFileSuccess = "Json file was successfully minified.";
    public static string MultipleFileSuccess(int fileCount) => $"Folder with {fileCount} files successfully minified.";

    public static string MinifyFileTitle(string filePath) => $"[purple]Minifying[/] the JSON file: [yellow]'{filePath}'[/]";

    public static string MinifyFolderTitle(string folderPath) => $"[purple]Minifying[/] the JSON files in folder: [yellow]'{folderPath}'[/]";

    public static string MinifyFolderLineItem(string filePath) => $" - Minifying file: [yellow]{filePath}[/]";

    public static class Errors
    {
        public static Error FilePathRequired = Error.Validation("FilePath.Required", "The file path is required.");
        public static Error FolderPathRequired = Error.Validation("FolderPath.Required", "The folder path is required.");

        public static Error FilePathNotFound(string path) =>
            Error.NotFound("FilePath.NotFound", $"The file '{path}' does not exist.");

        public static Error FilePathInvalidExtension(string path) =>
            Error.Validation("FilePath.Extension", $"The file '{path}' is not a json file.");

        public static Error FolderPathNotFound(string path) =>
            Error.NotFound("FolderPath.NotFound", $"The folder '{path}' does not exist.");
    }
}
