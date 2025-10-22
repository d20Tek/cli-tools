using D20Tek.NuGet.Portfolio.Domain;
using D20Tek.NuGet.Portfolio.Persistence;

namespace D20Tek.NuGet.Portfolio.Features.TrackedPackages;

internal sealed class ListTrackedPackagesCommand(IAnsiConsole console, AppDbContext dbContext) : Command
{
    private readonly IAnsiConsole _console = console;
    private readonly AppDbContext _dbContext = dbContext;

    public override int Execute(CommandContext context) =>
        GetTrackedPackages()
            .Iter(RenderTrackedPackages)
            .Render(_console, s => $"{s.Length} packages retrieved.");

    private Result<TrackedPackageEntity[]> GetTrackedPackages() =>
        Try.Run(() =>
        {
            var packs = _dbContext.GetAllTrackedPackages();
            return Result<TrackedPackageEntity[]>.Success(packs);
        });

    private void RenderTrackedPackages(TrackedPackageEntity[] packages) =>
        _console.ToIdentity()
                .Iter(c => c.RenderTableWithTitle(
                   "List of Packages Tracked",
                   PackageTableBuilder.Create()
                                      .WithHeader()
                                      .WithRows(packages)
                                      .Build()));
}
