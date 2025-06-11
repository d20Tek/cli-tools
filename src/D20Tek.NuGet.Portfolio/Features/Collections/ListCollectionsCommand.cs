using D20Tek.NuGet.Portfolio.Common;
using D20Tek.NuGet.Portfolio.Common.Controls;
using D20Tek.NuGet.Portfolio.Domain;
using D20Tek.NuGet.Portfolio.Persistence;
using Microsoft.EntityFrameworkCore;

namespace D20Tek.NuGet.Portfolio.Features.Collections;

internal sealed class ListCollectionsCommand : AsyncCommand
{
    private readonly IAnsiConsole _console;
    private readonly AppDbContext _dbContext;

    public ListCollectionsCommand(IAnsiConsole console, AppDbContext dbContext) =>
        (_console, _dbContext) = (console, dbContext);

    public override Task<int> ExecuteAsync(CommandContext context) =>
        GetCollections()
            .IterAsync(RenderCollections)
            .RenderAsync(_console, s => $"{s.Length} collections retrieved.");

    private async Task<Result<CollectionEntity[]>> GetCollections() =>
        await TryAsync.RunAsync(async () =>
        {
            var colls = await _dbContext.Collections.ToArrayAsync();
            return Result<CollectionEntity[]>.Success(colls);
        });

    private Task RenderCollections(CollectionEntity[] collections) =>
        _console.ToIdentity()
                .Iter(c => c.WriteLine())
                .Iter(c => c.CommandHeader().Render("List of Collections"))
                .Iter(c => c.Write(CollectionTableBuilder.Create()
                                                         .WithHeader()
                                                         .WithRows(collections)
                                                         .Build()))
                .Map(_ => Task.CompletedTask);
}
