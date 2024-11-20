using CommerceMicro.Modules.Contracts;
using CommerceMicro.ProductService.Application.Data;
using CommerceMicro.ProductService.Application.Users.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace CommerceMicro.ProductService.Application.Users.Consumers;

public class UserCreatedForProductConsumer : IConsumer<UserCreatedEvent>
{
	private readonly AppDbContext _appDbContext;

	public UserCreatedForProductConsumer(
		AppDbContext appDbContext)
	{
		_appDbContext = appDbContext;
	}

	public async Task Consume(ConsumeContext<UserCreatedEvent> context)
	{
		var userFound = await _appDbContext.Users.FirstOrDefaultAsync(x => x.Id == context.Message.Id, context.CancellationToken);

		if (userFound is null)
		{
			var newUser = new User()
			{
				Id = context.Message.Id,
				FirstName = context.Message.FirstName,
				LastName = context.Message.LastName,
				UserName = context.Message.UserName
			};

			await _appDbContext.Users.AddAsync(newUser, cancellationToken: context.CancellationToken);

			await _appDbContext.SaveChangesAsync();
		}
	}
}
