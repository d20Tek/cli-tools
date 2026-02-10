using D20Tek.Tools.Common.Controls;
using D20Tek.Tools.JsonMinify.Services;
using System.Text.Json;

namespace D20Tek.Tools.JsonMinify.Commands;

internal sealed class MinifyFileCommand(IMinifyService minifyService, IAnsiConsole console)
    : Command<MinifyFileCommand.Settings>
{
    private static readonly JsonSerializerOptions _options = new() { WriteIndented = false };
    private readonly IMinifyService _minifyService = minifyService;
    private readonly IAnsiConsole _console = console;

    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<FILEPATH>")]
        [Description("The fully qualified file path for the target json file to minify.")]
        public string FilePath { get; init; } = string.Empty;
    }

    public override int Execute(CommandContext context, Settings settings, CancellationToken _) =>
        _minifyService.MinifyFile(settings.FilePath)
                      .Render(_console, _ => Constants.SingleFileSuccess);
}
