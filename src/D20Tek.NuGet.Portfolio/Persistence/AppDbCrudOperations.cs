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
}
