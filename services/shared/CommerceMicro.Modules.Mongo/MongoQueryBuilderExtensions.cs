using System.Linq.Expressions;
using CommerceMicro.Modules.Core.Pagination;
using MongoDB.Driver;

namespace CommerceMicro.Modules.Mongo;

public static class MongoQueryBuilderExtensions
{
	public static IFindFluent<T, T> SortBy<T>(
		this IFindFluent<T, T> query,
		string sorting)
	{
		if (query == null)
			throw new ArgumentNullException(nameof(query));

		if (string.IsNullOrEmpty(sorting))
		{
			return query;
		}

		var builder = Builders<T>.Sort;

		var sortDefinition = sorting.Contains("desc", StringComparison.OrdinalIgnoreCase) ?
			builder.Descending(sorting.ToLower().Replace(" desc", "")) :
			builder.Ascending(sorting.ToLower().Replace(" asc", ""));

		return query.Sort(sortDefinition);
	}

	public static IFindFluent<T, T> PageBy<T>(
		this IFindFluent<T, T> query,
		IPageRequest pagedResultRequest)
	{
		if (query == null)
			throw new ArgumentNullException(nameof(query));

		return query.Skip(pagedResultRequest.SkipCount).Limit(pagedResultRequest.MaxResultCount);
	}

	public static Task<T> FirstOrDefaultAsync<T>(
		this IMongoCollection<T> query,
		Expression<Func<T, bool>> predicate,
		CancellationToken cancellationToken = default
	)
	{
		var filter = Builders<T>.Filter.Where(predicate);

		return query.Find(filter).FirstOrDefaultAsync(cancellationToken);
	}

	public static async Task<bool> AnyAsync<T>(
		this IMongoCollection<T> query,
		Expression<Func<T, bool>> predicate,
		CancellationToken cancellationToken = default
	)
	{
		var filter = Builders<T>.Filter.Where(predicate);
		var count = await query.Find(filter).CountDocumentsAsync(cancellationToken);

		return count > 0;
	}

	public static async Task<long> DeleteAsync<T>(
		this IMongoCollection<T> query,
		Expression<Func<T, bool>> predicate,
		CancellationToken cancellationToken = default
	)
	{
		var filter = Builders<T>.Filter.Where(predicate);
		var count = await query.DeleteManyAsync(filter, cancellationToken);

		return count?.DeletedCount ?? 0;
	}
}