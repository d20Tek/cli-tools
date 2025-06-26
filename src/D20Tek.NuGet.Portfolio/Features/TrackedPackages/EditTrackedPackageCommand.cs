using D20Tek.NuGet.Portfolio.Domain;
using D20Tek.NuGet.Portfolio.Persistence;

namespace D20Tek.NuGet.Portfolio.Features.TrackedPackages;

internal class EditTrackedPackageCommand : AsyncCommand<EditTrackedPackageCommand.Request>
{
    internal sealed class Request : CommandSettings
    {
        [CommandOption("-i|--id")]
        [Description("The numeric id of the tracked package to edit.")]
        public int Id { get; set; }

        [CommandOption("-p|--package-id")]
        [Description("The tracked package's NuGet package id.")]
        public string PackageId { get; set; } = "";

        [CommandOption("-c|--collection-id")]
        [Description("The id of the collection this package belongs to.")]
        public int CollectionId { get; set; }
    }

    private readonly IAnsiConsole _console;
    private readonly AppDbContext _dbContext;

    public EditTrackedPackageCommand(IAnsiConsole console, AppDbContext dbContext) =>
        (_console, _dbContext) = (console, dbContext);

    public override async Task<int> ExecuteAsync(CommandContext context, Request request)
    {
        _console.CommandHeader().Render("Edit collection");
        return await GetEntity(request.Id)
            .Bind(entity => GetRequestInput(request, entity)
                                .Pipe(input => GetCollection(input.CollectionId)
                                .Bind(_ => entity.Update(input.PackageId, input.CollectionId))))
            .BindAsync(UpdateEntity)
            .RenderAsync(_console, s => $"Updated collection: '{s.PackageId}' [Id: {s.Id}].");
    }

    private Request GetRequestInput(Request request, TrackedPackageEntity prevEntity) =>
        request.ToIdentity()
               .Iter(r => r.PackageId = _console.PromptIfDefault(r.PackageId, "Update NuGet package id:", prevEntity.PackageId))
               .Iter(r => r.CollectionId = _console.PromptIfDefault(r.CollectionId, "Update its collection id:", prevEntity.CollectionId));

    private Result<TrackedPackageEntity> GetEntity(int id) =>
        _console.AskIfDefault(id, "Id of tracked package to edit:")
                .Pipe(i => _dbContext.TrackedPackages.FirstOrDefault(c => c.Id == i)?
                           .Pipe(Result<TrackedPackageEntity>.Success)
                               ?? Result<TrackedPackageEntity>.Failure(Errors.EntityNotFound(nameof(TrackedPackageEntity), i)));

    private Result<CollectionEntity> GetCollection(int id) =>
        _dbContext.Collections.FirstOrDefault(c => c.Id == id)?
            .Pipe(Result<CollectionEntity>.Success)
                ?? Result<CollectionEntity>.Failure(Errors.EntityNotFound(nameof(CollectionEntity), id));

    private async Task<Result<TrackedPackageEntity>> UpdateEntity(TrackedPackageEntity entity) =>
        await TryAsync.RunAsync(async () =>
        {
            await _dbContext.SaveChangesAsync();
            return Result<TrackedPackageEntity>.Success(entity);
        });
}
