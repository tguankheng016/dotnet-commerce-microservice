using CommerceMicro.CartService.Application.Data;
using CommerceMicro.CartService.Application.Products.Models;
using CommerceMicro.Modules.Contracts;
using MassTransit;
using MongoDB.Driver;

namespace CommerceMicro.CartService.Application.Products.Consumers;

public class ProductDeletedForCartConsumer : IConsumer<ProductDeletedEvent>
{
	private readonly AppDbContext _appDbContext;

	public ProductDeletedForCartConsumer(
		AppDbContext appDbContext)
	{
		_appDbContext = appDbContext;
	}

	public async Task Consume(ConsumeContext<ProductDeletedEvent> context)
	{
		var filter = Builders<Product>.Filter.Where(x => x.Id == context.Message.Id);

		await _appDbContext.Products.DeleteOneAsync(filter, context.CancellationToken);
	}
}
