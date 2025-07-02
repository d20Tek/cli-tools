using D20Tek.NuGet.Portfolio.Domain;
using Microsoft.EntityFrameworkCore;

namespace D20Tek.NuGet.Portfolio.Persistence;

internal static class AppDbCrudOperations
{
    public static async Task<Result<T>> DeleteEntity<T>(this DbSet<T> set, T entity, AppDbContext context)
        where T : class, IEntity
    {
        set.Remove(entity);
        await context.SaveChangesAsync();
        return Result<T>.Success(entity);
    }
}
