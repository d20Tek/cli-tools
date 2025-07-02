using D20Tek.NuGet.Portfolio.Persistence;

namespace D20Tek.NuGet.Portfolio.Features.TrackedPackages;

internal class DeleteTrackedPackageCommand : AsyncCommand<DeleteTrackedPackageCommand.PackageId>
{
    public sealed class PackageId : CommandSettings
    {
        [CommandOption("-i|--id")]
        [Description("The numeric id of the tracked package to delete.")]
        public int Value { get; set; }
    }

    private readonly IAnsiConsole _console;
    private readonly AppDbContext _dbContext;

    public DeleteTrackedPackageCommand(IAnsiConsole console, AppDbContext dbContext) =>
        (_console, _dbContext) = (console, dbContext);

    public override async Task<int> ExecuteAsync(CommandContext context, PackageId id)
    {
        _console.CommandHeader().Render("Delete tracked package");
        return await id.Pipe(i => EnsureIdInput(i))
                       .Pipe(i => _dbContext.TrackedPackages.DeleteEntityById(i.Value, _dbContext))
                       .RenderAsync(_console, s => $"Tracked package deleted: '{s.PackageId}' [Id: {s.Id}].");
    }

    private PackageId EnsureIdInput(Identity<PackageId> id) =>
        id.Iter(r => r.Value = _console.AskIfDefault(r.Value, "Enter the tracked package id:"));
}
