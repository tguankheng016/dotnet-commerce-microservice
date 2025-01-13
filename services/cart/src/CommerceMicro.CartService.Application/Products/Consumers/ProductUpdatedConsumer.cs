using CommerceMicro.CartService.Application.Data;
using CommerceMicro.CartService.Application.Products.Models;
using CommerceMicro.Modules.Contracts;
using CommerceMicro.Modules.Mongo;
using MassTransit;
using MongoDB.Driver;

namespace CommerceMicro.CartService.Application.Products.Consumers;

public class ProductUpdatedForCartConsumer : IConsumer<ProductUpdatedEvent>
{
	private readonly AppDbContext _appDbContext;

	public ProductUpdatedForCartConsumer(
		AppDbContext appDbContext)
	{
		_appDbContext = appDbContext;
	}

	public async Task Consume(ConsumeContext<ProductUpdatedEvent> context)
	{
		var productFound = await _appDbContext.Products.FirstOrDefaultAsync(x => x.Id == context.Message.Id, context.CancellationToken);

		if (productFound is null)
		{
			var newProduct = new Product()
			{
				Id = context.Message.Id,
				Name = context.Message.Name,
				Description = context.Message.Description,
				Price = context.Message.Price,
				StockQuantity = context.Message.StockQuantity
			};

			await _appDbContext.Products.InsertOneAsync(newProduct, cancellationToken: context.CancellationToken);
		}
		else
		{
			await _appDbContext.Products.UpdateOneAsync(
					x => x.Id == context.Message.Id,
					Builders<Product>.Update
						.Set(x => x.Name, context.Message.Name)
						.Set(x => x.Description, context.Message.Description)
						.Set(x => x.Price, context.Message.Price)
						.Set(x => x.StockQuantity, context.Message.StockQuantity)
				);
		}
	}
}
