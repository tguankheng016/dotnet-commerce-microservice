using CommerceMicro.Modules.Core.Persistences;
using CommerceMicro.ProductService.Application.Categories.Models;

namespace CommerceMicro.ProductService.Application.Data.Seed;

public class DataSeeder : IDataSeeder
{
	private readonly AppDbContext _appDbContext;

	public DataSeeder(AppDbContext appDbContext)
	{
		_appDbContext = appDbContext;
	}

	public async Task SeedAllAsync()
	{
		await SeedCategoryAsync();
	}

	public async Task SeedCategoryAsync()
	{
		var categories = new List<Category>
		{
			new Category { CategoryName = "Furniture", Id = 0 },
			new Category { CategoryName = "Clothing", Id = 0 },
			new Category { CategoryName = "Electronics", Id = 0 },
			new Category { CategoryName = "Travel", Id = 0 },
			new Category { CategoryName = "Books", Id = 0 },
			new Category { CategoryName = "Kitchen", Id = 0 },
		};

		await _appDbContext.Categories.AddRangeAsync(categories);
		await _appDbContext.SaveChangesAsync();
	}
}
