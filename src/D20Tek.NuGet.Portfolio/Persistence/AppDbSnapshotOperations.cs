using D20Tek.NuGet.Portfolio.Domain;
using Microsoft.EntityFrameworkCore;

namespace D20Tek.NuGet.Portfolio.Persistence;

internal static class AppDbSnapshotOperations
{
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

    public static async Task<Result<int>> DeleteSnapshotsByDate(
        this AppDbContext context,
        int collectionId,
        DateOnly snapshotDate) =>
        await TryAsync.RunAsync(() =>
            context.GetTrackPackagesByCollectionId(collectionId)
                   .SelectMany(x => context.PackageSnapshots
                                           .Where(y => y.TrackedPackageId == x.Id && y.SnapshotDate == snapshotDate))
                                           .ToIdentity()
                   .Iter(snapshots => snapshots.ForEach(s => context.PackageSnapshots.Remove(s)))
                   .Pipe(async _ =>
                   {
                       var changes = await context.SaveChangesAsync();
                       return Result<int>.Success(changes);
                   }));

    public static Result<PackageSnapshotEntity[]> GetSnapshotsForCollection(
        this AppDbContext context,
        int collectionId,
        DateOnly snapshotDate) =>
        Try.Run<PackageSnapshotEntity[]>(() =>
            context.GetTrackPackagesByCollectionId(collectionId)
                   .SelectMany(x => context.PackageSnapshots
                                           .Where(y => y.TrackedPackageId == x.Id && y.SnapshotDate == snapshotDate)
                                           .Include(i => i.TrackedPackage))
                   .ToArray());

    public static Result<PackageSnapshotEntity[]> GetSnapshotsForCollection(
        this AppDbContext context,
        int collectionId,
        DateRange dateRange) =>
        Try.Run<PackageSnapshotEntity[]>(() =>
            context.GetTrackPackagesByCollectionId(collectionId)
                   .SelectMany(x => context.PackageSnapshots
                                           .Where(y => y.TrackedPackageId == x.Id && 
                                                      (y.SnapshotDate >= dateRange.Start && y.SnapshotDate <= dateRange.End))
                                           .Include(i => i.TrackedPackage))
                   .ToArray());
}
