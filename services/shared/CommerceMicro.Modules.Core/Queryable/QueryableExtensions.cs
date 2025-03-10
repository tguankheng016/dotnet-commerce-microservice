using System.Linq.Expressions;
using CommerceMicro.Modules.Core.Pagination;

namespace CommerceMicro.Modules.Core.Queryable;

public static class QueryableExtensions
{
    public static IQueryable<T> PageBy<T>(
        this IQueryable<T> query,
        int skipCount,
        int maxResultCount)
    {
        if (query is null)
            throw new ArgumentNullException(nameof(query));

        return query.Skip<T>(skipCount).Take<T>(maxResultCount);
    }

    public static IQueryable<T> PageBy<T>(
        this IQueryable<T> query,
        IPageRequest pagedResultRequest)
    {
        return query.PageBy<T>(pagedResultRequest.SkipCount ?? 0, pagedResultRequest.MaxResultCount ?? 10);
    }

    public static IQueryable<T> WhereIf<T>(
        this IQueryable<T> query,
        bool condition,
        Expression<Func<T, bool>> predicate)
    {
        return !condition ? query : query.Where<T>(predicate);
    }

    public static IQueryable<T> WhereIf<T>(
        this IQueryable<T> query,
        bool condition,
        Expression<Func<T, int, bool>> predicate)
    {
        return !condition ? query : query.Where<T>(predicate);
    }
}
