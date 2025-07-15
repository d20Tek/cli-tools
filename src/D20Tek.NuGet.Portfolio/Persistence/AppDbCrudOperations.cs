using D20Tek.NuGet.Portfolio.Domain;
using Microsoft.EntityFrameworkCore;

namespace D20Tek.NuGet.Portfolio.Persistence;

internal static class AppDbCrudOperations
{
    public static async Task<Result<T>> CreateEntity<T>(this DbSet<T> set, T entity, AppDbContext context)
        where T : class, IEntity =>
        await TryAsync.RunAsync(async () =>
        {
            var result = set.Add(entity);
            await context.SaveChangesAsync();
            return Result<T>.Success(result.Entity);
        });


    public static async Task<Result<T>> UpdateEntity<T>(this DbSet<T> set, T entity, AppDbContext context)
        where T : class, IEntity =>
        await TryAsync.RunAsync(async () =>
        {
            await context.SaveChangesAsync();
            return Result<T>.Success(entity);
        });

    public static async Task<Result<T>> DeleteEntityById<T>(this DbSet<T> set, int id, AppDbContext context)
        where T : class, IEntity =>
        await TryAsync.RunAsync(() =>
            set.GetEntityById(id)
               .BindAsync(async entity =>
               {
                   set.Remove(entity);
                   await context.SaveChangesAsync();
                   return Result<T>.Success(entity);
               }));

    public static async Task<Result<PackageSnapshotEntity[]>> UpsertSnapshots(
        this AppDbContext context,
        PackageSnapshotEntity[] snapshots) =>
        await TryAsync.RunAsync(async () =>
        {
            snapshots.ForEach(snapshot =>
                context.GetSnapshotByDate(snapshot.SnapshotDate)
                       .Match(
                            s => s.ChangeDownloads(snapshot.Downloads),
                            () => context.PackageSnapshots.Add(snapshot).Entity));

            await context.SaveChangesAsync();
            return Result<PackageSnapshotEntity[]>.Success(snapshots);
        });

    private static Option<PackageSnapshotEntity> GetSnapshotByDate(this AppDbContext context, DateOnly snapshotDate) =>
        context.PackageSnapshots.FirstOrDefault(x => x.SnapshotDate == snapshotDate).ToOption();

    public static async Task<Result<int>> DeleteSnapshotsByDate(this AppDbContext context, int collectionId, DateOnly snapshotDate) =>
        await TryAsync.RunAsync(() =>
            context.GetTrackPackagesByCollectionId(collectionId)
                   .SelectMany(x => context.PackageSnapshots.Where(y => y.TrackedPackageId == x.Id)).ToIdentity()
                   .Iter(snapshots => snapshots.ForEach(s => context.PackageSnapshots.Remove(s)))
                   .Pipe(async _ =>
                   {
                       var changes = await context.SaveChangesAsync();
                       return Result<int>.Success(changes);
                   }));
}
