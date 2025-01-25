using CommerceMicro.CartService.Application.Data;
using CommerceMicro.CartService.Application.Products.Models;
using CommerceMicro.Modules.Contracts;
using CommerceMicro.Modules.Mongo;
using MassTransit;

namespace CommerceMicro.CartService.Application.Products.Consumers;

public class ProductCreatedForCartConsumer : IConsumer<ProductCreatedEvent>
{
	private readonly AppDbContext _appDbContext;

	public ProductCreatedForCartConsumer(
		AppDbContext appDbContext)
	{
		_appDbContext = appDbContext;
	}

	public async Task Consume(ConsumeContext<ProductCreatedEvent> context)
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
	}
}
