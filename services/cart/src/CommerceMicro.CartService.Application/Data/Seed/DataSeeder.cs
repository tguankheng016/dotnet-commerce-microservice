using System.Security.Cryptography.X509Certificates;
using CommerceMicro.CartService.Application.Products.Models;
using CommerceMicro.Modules.Core.Persistences;
using CommerceMicro.Modules.Mongo;
using CommerceMicro.ProductService;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CommerceMicro.CartService.Application.Data.Seed;

public class DataSeeder : IDataSeeder
{
	private readonly AppDbContext _appDbContext;
	private readonly ProductGrpcService.ProductGrpcServiceClient _productGrpcServiceClient;

	public DataSeeder(
		AppDbContext appDbContext,
		ProductGrpcService.ProductGrpcServiceClient productGrpcServiceClient
	)
	{
		_appDbContext = appDbContext;
		_productGrpcServiceClient = productGrpcServiceClient;
	}

	public async Task SeedAllAsync()
	{
		var products = await _productGrpcServiceClient.GetAllProductsAsync(new GetAllProductsRequest());

		if (products is null) return;

		foreach (var product in products.Products)
		{
			var dbProduct = await _appDbContext.Products.FirstOrDefaultAsync(x => x.Id == product.Id);

			if (dbProduct is null)
			{
				var newProduct = new Product()
				{
					Id = product.Id,
					Name = product.Name,
					Description = product.Description,
					StockQuantity = product.StockQuantity,
					Price = (decimal)product.Price
				};

				await _appDbContext.Products.InsertOneAsync(newProduct);
			}
			else
			{
				await _appDbContext.Products.UpdateOneAsync(
					x => x.Id == product.Id,
					Builders<Product>.Update
						.Set(x => x.Name, product.Name)
						.Set(x => x.Description, product.Description)
						.Set(x => x.Price, (decimal)product.Price)
						.Set(x => x.StockQuantity, product.StockQuantity)
				);
			}
		}
	}
}
