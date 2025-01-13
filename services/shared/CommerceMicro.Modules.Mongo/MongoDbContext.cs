using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace CommerceMicro.Modules.Mongo;

public class MongoDbContextBase : ICustomMongoDbContext
{
	public IClientSessionHandle? Session { get; set; }

	public IMongoDatabase Database { get; }

	public IMongoClient MongoClient { get; }

	private readonly ILogger<MongoDbContextBase>? _logger;

	private readonly SemaphoreSlim _transactionSemaphore = new SemaphoreSlim(0);

	public MongoDbContextBase(
		IMongoClient mongoClient,
		IMongoDatabase mongoDatabase,
		ILogger<MongoDbContextBase>? logger = null
	)
	{
		RegisterConventions();

		MongoClient = mongoClient;
		Database = mongoDatabase;
		_logger = logger;
	}

	private static void RegisterConventions()
	{
		ConventionRegistry.Register(
			"conventions",
			new ConventionPack
			{
				new CamelCaseElementNameConvention(),
				new IgnoreExtraElementsConvention(true),
				new EnumRepresentationConvention(BsonType.String),
				new IgnoreIfDefaultConvention(false),
			}, _ => true);
	}

	public IMongoCollection<T> GetCollection<T>(string? name = null)
	{
		return Database.GetCollection<T>(name ?? typeof(T).Name.ToLower());
	}

	public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		using (Session = await MongoClient.StartSessionAsync(cancellationToken: cancellationToken))
		{
			Session.StartTransaction();

			try
			{
				await Session.CommitTransactionAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				_logger?.LogError(ex, "Transaction failed");
				await Session.AbortTransactionAsync(cancellationToken);
				throw;
			}
		}

		return 1;
	}

	public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
	{
		Session = await MongoClient.StartSessionAsync(cancellationToken: cancellationToken);
		Session.StartTransaction();
	}

	public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
	{
		if (Session is { IsInTransaction: true })
		{
			await Session.CommitTransactionAsync(cancellationToken);

			_transactionSemaphore.Release();
		}

		Session?.Dispose();
	}

	public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
	{
		await Session?.AbortTransactionAsync(cancellationToken)!;
	}

	public void Dispose()
	{
		if (Session is { IsInTransaction: true })
		{
			_transactionSemaphore.Wait();
		}

		GC.SuppressFinalize(this);
	}
}

public interface ICustomMongoDbContext : IDisposable
{
	IMongoDatabase Database { get; }

	IMongoClient MongoClient { get; }

	IMongoCollection<T> GetCollection<T>(string? name = null);

	Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

	Task BeginTransactionAsync(CancellationToken cancellationToken = default);

	Task CommitTransactionAsync(CancellationToken cancellationToken = default);

	Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
