namespace D20Tek.NuGet.Portfolio.Features.Collections;

internal sealed class AddCollectionCommand : AsyncCommand<AddCollectionCommand.Request>
{
    private readonly IAnsiConsole _console;

    public AddCollectionCommand(IAnsiConsole console) => _console = console;

    internal class Request : CommandSettings
    {
        [CommandOption("-n|--name")]
        [Description("The collection's name property.")]
        public string Name { get; set; } = "";
    }

    public override Task<int> ExecuteAsync(CommandContext context, Request request)
    {
        _console.WriteLine("Add new collection");
        _console.WriteLine("------------------");

        if (string.IsNullOrEmpty(request.Name))
        {
            _console.WriteLine("Enter the new collection's name:");
            request.Name = _console.Ask<string>(Globals.AppPrompt);
        }

        _console.MarkupLine($"[green]Success![/] Created a new collection: '{request.Name}'");
        return Task.FromResult(Globals.S_OK);
    }
}
