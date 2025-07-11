﻿using D20Tek.NuGet.Portfolio.Abstractions;
using D20Tek.NuGet.Portfolio.Features.PackageDownloads;
using D20Tek.NuGet.Portfolio.Persistence;

namespace D20Tek.NuGet.Portfolio.Features.PackageDownloads;

internal sealed class GetDownloadsByCollectionIdCommand : AsyncCommand<GetDownloadsByCollectionIdCommand.CollectionId>
{
    public sealed class CollectionId : CommandSettings
    {
        [CommandOption("-c|--collection-id")]
        [Description("The numeric id of the collection of tracked packages to retrieve.")]
        public int Value { get; set; }
    }

    private readonly IAnsiConsole _console;
    private readonly AppDbContext _dbContext;
    private readonly INuGetSearchClient _client;

    public GetDownloadsByCollectionIdCommand(
        IAnsiConsole console,
        AppDbContext dbContext,
        INuGetSearchClient client) =>
        (_console, _dbContext, _client) = (console, dbContext, client);

    public override async Task<int> ExecuteAsync(CommandContext context, CollectionId id)
    {
        _console.CommandHeader().Render("Get collection package downloads");
        return await id.Pipe(i => EnsureIdInput(i))
                       .Pipe(i => _dbContext.Collections.GetEntityById(i.Value)
                           .Map(_ => _dbContext.GetTrackPackagesByCollectionId(i.Value)))
                       .BindAsync(p => _client.RetrieveDownloadSnapshots(p))
                       .IterAsync(s => _console.RenderDownloadSnapshots(s))
                       .RenderAsync(_console, s => $"Retrieved downloads for {s.Length} tracked packages.");
    }

    private CollectionId EnsureIdInput(Identity<CollectionId> id) =>
        id.Iter(r => r.Value = _console.AskIfDefault(r.Value, "Enter the collection id:"));
}
