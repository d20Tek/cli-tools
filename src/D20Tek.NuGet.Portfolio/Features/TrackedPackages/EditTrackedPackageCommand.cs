using D20Tek.NuGet.Portfolio.Domain;
using D20Tek.NuGet.Portfolio.Persistence;

namespace D20Tek.NuGet.Portfolio.Features.TrackedPackages;

internal class EditTrackedPackageCommand(IAnsiConsole console, AppDbContext dbContext) :
    AsyncCommand<EditTrackedPackageCommand.Request>
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

    private readonly IAnsiConsole _console = console;
    private readonly AppDbContext _dbContext = dbContext;

    public override async Task<int> ExecuteAsync(CommandContext context, Request request)
    {
        _console.CommandHeader().Render("Edit collection");
        return await GetEntity(request.Id)
            .Bind(entity => GetRequestInput(request, entity)
                                .Pipe(input => entity.Update(input.PackageId, input.CollectionId)))
            .BindAsync(EditEntity)
            .RenderAsync(_console, s => $"Updated collection: '{s.PackageId}' [Id: {s.Id}].");
    }

    private Request GetRequestInput(Request request, TrackedPackageEntity prevEntity) =>
        request.ToIdentity()
               .Iter(r => r.PackageId = _console.PromptIfDefault(r.PackageId, "Update NuGet package id:", prevEntity.PackageId))
               .Iter(r => r.CollectionId = _console.PromptIfDefault(r.CollectionId, "Update its collection id:", prevEntity.CollectionId));

    private Result<TrackedPackageEntity> GetEntity(int id) =>
        _console.AskIfDefault(id, "Id of tracked package to edit:")
                .Pipe(i => _dbContext.TrackedPackages.GetEntityById(i));

    private async Task<Result<TrackedPackageEntity>> EditEntity(TrackedPackageEntity entity) =>
        await _dbContext.Collections.GetEntityById(entity.CollectionId)
                                    .BindAsync(coll => _dbContext.TrackedPackages.UpdateEntity(entity, _dbContext));
}
