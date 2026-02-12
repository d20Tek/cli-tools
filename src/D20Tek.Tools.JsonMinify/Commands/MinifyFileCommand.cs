using D20Tek.Spectre.Console.Extensions.Controls;
using D20Tek.Tools.Common.Controls;
using D20Tek.Tools.JsonMinify.Services;

namespace D20Tek.Tools.JsonMinify.Commands;

internal sealed class MinifyFileCommand(IFileSystemAdapter fileAdapter, IMinifyService minifyService, IAnsiConsole console)
    : Command<MinifyFileCommand.Settings>
{
    private readonly IFileSystemAdapter _fileAdapter = fileAdapter;
    private readonly IMinifyService _minifyService = minifyService;
    private readonly IAnsiConsole _console = console;
    private readonly FilePathValidator _validator = new(fileAdapter);

    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<FILEPATH>")]
        [Description("The fully qualified file path for the target json file to minify.")]
        public string FilePath { get; init; } = string.Empty;
    }

    public override int Execute(CommandContext context, Settings settings, CancellationToken _) =>
        _validator.Validate(settings.FilePath)
                  .Iter(f => _console.WriteMessages(Constants.MinifyFileTitle(f)))
                  .Bind(f => _minifyService.MinifyFile(settings.FilePath))
                  .Render(_console, _ => Constants.SingleFileSuccess);
}
