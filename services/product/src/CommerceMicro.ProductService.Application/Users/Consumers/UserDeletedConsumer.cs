using CommerceMicro.Modules.Contracts;
using CommerceMicro.ProductService.Application.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace CommerceMicro.ProductService.Application.Users.Consumers;

public class UserDeletedForProductConsumer : IConsumer<UserDeletedEvent>
{
	private readonly AppDbContext _appDbContext;

	public UserDeletedForProductConsumer(
		AppDbContext appDbContext)
	{
		_appDbContext = appDbContext;
	}

	public async Task Consume(ConsumeContext<UserDeletedEvent> context)
	{
		var userFound = await _appDbContext.Users.FirstOrDefaultAsync(x => x.Id == context.Message.Id, context.CancellationToken);

		if (userFound is not null)
		{
			_appDbContext.Users.Remove(userFound);
			await _appDbContext.SaveChangesAsync();
		}
	}
}
