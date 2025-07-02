using D20Tek.NuGet.Portfolio.Domain;
using D20Tek.NuGet.Portfolio.Persistence;
using Microsoft.EntityFrameworkCore;

namespace D20Tek.NuGet.Portfolio.Features.TrackedPackages;

internal sealed class ListTrackedPackagesCommand : AsyncCommand
{
    private readonly IAnsiConsole _console;
    private readonly AppDbContext _dbContext;

    public ListTrackedPackagesCommand(IAnsiConsole console, AppDbContext dbContext) =>
        (_console, _dbContext) = (console, dbContext);

    public override Task<int> ExecuteAsync(CommandContext context) =>
        GetTrackedPackages()
            .IterAsync(RenderTrackedPackages)
            .RenderAsync(_console, s => $"{s.Length} packages retrieved.");

    private async Task<Result<TrackedPackageEntity[]>> GetTrackedPackages() =>
        await TryAsync.RunAsync(async () =>
        {
            var packs = await _dbContext.TrackedPackages.AsNoTracking()
                                                        .Include(x => x.Collection)
                                                        .ToArrayAsync();
            return Result<TrackedPackageEntity[]>.Success(packs);
        });

    private Task RenderTrackedPackages(TrackedPackageEntity[] packages) =>
        _console.ToIdentity()
                .Iter(c => c.RenderTableWithTitle(
                   "List of Packages Tracked",
                   PackageTableBuilder.Create()
                                      .WithHeader()
                                      .WithRows(packages)
                                      .Build()))
                .Map(_ => Task.CompletedTask);
}
