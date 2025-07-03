using D20Tek.NuGet.Portfolio.Domain;
using D20Tek.NuGet.Portfolio.Persistence;

namespace D20Tek.NuGet.Portfolio.Features.Collections;

internal sealed class ListCollectionsCommand : Command
{
    private readonly IAnsiConsole _console;
    private readonly AppDbContext _dbContext;

    public ListCollectionsCommand(IAnsiConsole console, AppDbContext dbContext) =>
        (_console, _dbContext) = (console, dbContext);

    public override int Execute(CommandContext context) =>
        GetCollections()
            .Iter(RenderCollections)
            .Render(_console, s => $"{s.Length} collections retrieved.");

    private Result<CollectionEntity[]> GetCollections() =>
        Try.Run(() =>
        {
            var colls = _dbContext.GetAllCollections();
            return Result<CollectionEntity[]>.Success(colls);
        });

    private void RenderCollections(CollectionEntity[] collections) =>
        _console.ToIdentity()
                .Iter(c => c.RenderTableWithTitle(
                   "List of collections",
                   CollectionTableBuilder.Create()
                                         .WithHeader()
                                         .WithRows(collections)
                                         .Build()));
}
