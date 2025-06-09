namespace D20Tek.NuGet.Portfolio.Features.Collections;

internal class AddCollectionCommand : AsyncCommand<AddCollectionCommand.Request>
{
    internal class Request : CommandSettings
    {
        [CommandOption("-n|--name")]
        [Description("The collection's name property.")]
        public string Name { get; set; } = "";
    }

    public override Task<int> ExecuteAsync(CommandContext context, Request settings)
    {
        AnsiConsole.WriteLine("Add new collection");
        return Task.FromResult(Globals.S_OK);
    }
}
