using D20Tek.NuGet.Portfolio.Abstractions;
using D20Tek.NuGet.Portfolio.Domain;
using D20Tek.NuGet.Portfolio.Persistence;

namespace D20Tek.NuGet.Portfolio.Features.PackageDownloads;

internal class GetDownloadsByPackageIdCommand : AsyncCommand<GetDownloadsByPackageIdCommand.PackageId>
{
    public sealed class PackageId : CommandSettings
    {
        [CommandOption("-i|--id")]
        [Description("The numeric id of the tracked package to get download count for.")]
        public int Value { get; set; }
    }

    private readonly IAnsiConsole _console;
    private readonly AppDbContext _dbContext;
    private readonly INuGetSearchClient _client;

    public GetDownloadsByPackageIdCommand(
        IAnsiConsole console,
        AppDbContext dbContext,
        INuGetSearchClient client) =>
        (_console, _dbContext, _client) = (console, dbContext, client);

    public override async Task<int> ExecuteAsync(CommandContext context, PackageId id)
    {
        _console.CommandHeader().Render("Get current package downloads");
        return await id.Pipe(i => EnsureIdInput(i))
                       .Pipe(i => GetTrackedPackage(i))
                       .BindAsync(p => RetrieveDownloadSnapshot(p))
                       .RenderAsync(_console, s => $"Package - '{s.TrackedPackage.PackageId}': {s.Downloads} downloads.");
    }

    private PackageId EnsureIdInput(Identity<PackageId> id) =>
        id.Iter(r => r.Value = _console.AskIfDefault(r.Value, "Enter the tracked package id:"));

    private Result<TrackedPackageEntity> GetTrackedPackage(PackageId id) =>
        _dbContext.TrackedPackages.FirstOrDefault(p => p.Id == id.Value)?
            .Pipe(Result<TrackedPackageEntity>.Success)
                ?? Result<TrackedPackageEntity>.Failure(Errors.EntityNotFound(nameof(TrackedPackageEntity), id.Value));

    private async Task<Result<PackageSnapshotEntity>> RetrieveDownloadSnapshot(TrackedPackageEntity package)
    {
        var downloads = await _client.GetTotalDownloadsAsync(package.PackageId);
        return downloads.Map(d => PackageSnapshotEntity.Create(d, package));
    }
}
