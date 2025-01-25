using CommerceMicro.CartService.Application.Carts.Models;
using CommerceMicro.CartService.Application.Products.Models;
using CommerceMicro.Modules.Mongo;
using Humanizer;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace CommerceMicro.CartService.Application.Data;

public class AppDbContext : MongoDbContextBase
{
	public AppDbContext(
		IMongoClient mongoClient,
		IMongoDatabase mongoDatabase,
		ILogger<MongoDbContextBase>? logger = null
	) : base(mongoClient, mongoDatabase, logger)
	{
		Carts = GetCollection<Cart>(nameof(Carts).Underscore());
		Products = GetCollection<Product>(nameof(Products).Underscore());
	}

	public IMongoCollection<Cart> Carts { get; }

	public IMongoCollection<Product> Products { get; }
}
