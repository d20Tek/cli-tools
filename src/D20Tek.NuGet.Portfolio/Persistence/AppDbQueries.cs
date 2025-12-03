using D20Tek.NuGet.Portfolio.Domain;
using Microsoft.EntityFrameworkCore;

namespace D20Tek.NuGet.Portfolio.Persistence;

internal static class AppDbQueries
{
    public static Result<T> GetEntityById<T>(this DbSet<T> set, int id) where T : class, IEntity =>
        set.FirstOrDefault(x => x.Id == id)?
            .Pipe(Result<T>.Success) ?? Result<T>.Failure(Errors.EntityNotFound(typeof(T).Name, id));

    public static CollectionEntity[] GetAllCollections(this AppDbContext context) =>
        [.. context.Collections.AsNoTracking()];

    public static TrackedPackageEntity[] GetAllTrackedPackages(this AppDbContext context) =>
        [.. context.TrackedPackages.AsNoTracking().Include(x => x.Collection)];

    public static TrackedPackageEntity[] GetTrackPackagesByCollectionId(this AppDbContext context, int collectionId) =>
        [.. context.TrackedPackages.AsNoTracking().Where(x => x.CollectionId == collectionId)];

    public static TrackedPackageEntity[] GetTrackPackagesByCollectionIdAsTracking(this AppDbContext context, int collectionId) =>
        [.. context.TrackedPackages.Where(x => x.CollectionId == collectionId)];
}
