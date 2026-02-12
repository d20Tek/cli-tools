using D20Tek.Spectre.Console.Extensions.Controls;
using D20Tek.Tools.Common.Controls;
using D20Tek.Tools.JsonMinify.Services;

namespace D20Tek.Tools.JsonMinify.Commands;

internal sealed class MinifyFolderCommand(IFileSystemAdapter fileAdapter, IMinifyService minifyService, IAnsiConsole console)
    : Command<MinifyFolderCommand.Settings>
{
    private readonly IFileSystemAdapter _fileAdapter = fileAdapter;
    private readonly IMinifyService _minifyService = minifyService;
    private readonly IAnsiConsole _console = console;
    private readonly FolderPathValidator _validator = new(fileAdapter);

    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<FOLDERPATH>")]
        [Description("The fully qualified target directory/folder path to search for json files to minify.")]
        public string FolderPath { get; init; } = string.Empty;
    }

    public override int Execute(CommandContext context, Settings settings, CancellationToken _) =>
        _validator.Validate(settings.FolderPath)
                  .Iter(f => _console.WriteMessages(Constants.MinifyFolderTitle(f)))
                  .Map(f => _fileAdapter.EnumerateFolderFiles(f, Constants.JsonFileSearchPattern))
                  .Bind(fileSet => MinifyFileSet(fileSet))
                  .Render(_console, count => Constants.MultipleFileSuccess(count));

    private Result<int> MinifyFileSet(IEnumerable<string> filePaths) =>
        filePaths.Count(filePath =>
            filePath.Pipe(f => _console.WriteMessages(Constants.MinifyFolderLineItem(f)))
                    .Pipe(f => _minifyService.MinifyFile(f))
                    .IsSuccess);
}
