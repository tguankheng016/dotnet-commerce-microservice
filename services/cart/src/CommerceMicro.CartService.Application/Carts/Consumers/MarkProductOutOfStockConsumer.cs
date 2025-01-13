using CommerceMicro.CartService.Application.Carts.Models;
using CommerceMicro.CartService.Application.Data;
using CommerceMicro.CartService.Application.Products.Models;
using CommerceMicro.Modules.Contracts;
using MassTransit;
using MongoDB.Driver;

namespace CommerceMicro.CartService.Application.Carts.Consumers;

public class MarkProductOutOfStockConsumer : IConsumer<MarkProductOfStockEvent>
{
	private readonly AppDbContext _appDbContext;

	public MarkProductOutOfStockConsumer(AppDbContext appDbContext)
	{
		_appDbContext = appDbContext;
	}

	public async Task Consume(ConsumeContext<MarkProductOfStockEvent> context)
	{
		await _appDbContext.Carts.UpdateManyAsync(
			x => x.Product.Id == context.Message.Id,
			Builders<Cart>.Update
				.Set(x => x.IsOutOfStock, context.Message.StockQuantity <= 0)
		);

		await _appDbContext.Products.UpdateOneAsync(
			x => x.Id == context.Message.Id,
			Builders<Product>.Update
				.Set(x => x.StockQuantity, context.Message.StockQuantity)
		);
	}
}
