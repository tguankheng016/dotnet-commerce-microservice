using CommerceMicro.Modules.Contracts;
using CommerceMicro.ProductService.Application.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace CommerceMicro.ProductService.Application.Products.Consumers;

public class ChangeProductQuantityConsumer : IConsumer<ChangeProductQuantityEvent>
{
	private readonly AppDbContext _appDbContext;

	private readonly IPublishEndpoint _publishEndpoint;

	public ChangeProductQuantityConsumer(
		AppDbContext appDbContext,
		IPublishEndpoint publishEndpoint)
	{
		_appDbContext = appDbContext;
		_publishEndpoint = publishEndpoint;
	}

	public async Task Consume(ConsumeContext<ChangeProductQuantityEvent> context)
	{
		var product = await _appDbContext.Products
			.FirstOrDefaultAsync(x => x.Id == context.Message.Id, context.CancellationToken);

		if (product is not null)
		{
			product.StockQuantity += context.Message.QuantityChanged;

			if (product.StockQuantity < 0)
			{
				product.StockQuantity = 0;
			}

			await _publishEndpoint.Publish(
				new MarkProductOfStockEvent(
					product.Id,
					product.StockQuantity
				),
				context.CancellationToken
			);

			await _appDbContext.SaveChangesAsync(context.CancellationToken);
		}
	}
}
