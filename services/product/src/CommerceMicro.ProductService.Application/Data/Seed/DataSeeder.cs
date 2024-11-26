using CommerceMicro.Modules.Core.Persistences;
using CommerceMicro.ProductService.Application.Categories.Models;
using CommerceMicro.ProductService.Application.Products.Models;
using Microsoft.EntityFrameworkCore;

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
		await SeedProductAsync();
	}

	public async Task SeedCategoryAsync()
	{
		var existingCategories = await _appDbContext.Categories.CountAsync();

		if (existingCategories == 0)
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

	public async Task SeedProductAsync()
	{
		var existingProducts = await _appDbContext.Products.CountAsync();

		if (existingProducts == 0)
		{
			var products = new List<Product>
			{
				new Product
				{
					Id = 0,
					Name = "Modern Sofa",
					Description = "A stylish and comfortable sofa for your living room",
					Price = 1200,
					StockQuantity = 5,
					CategoryId = (await _appDbContext.Categories.FirstOrDefaultAsync(c => c.CategoryName == "Furniture"))?.Id
				},
				new Product
				{
					Id = 0,
					Name = "Summer Dress",
					Description = "A lightweight and stylish dress for the summer season",
					Price = 75,
					StockQuantity = 10,
					CategoryId = (await _appDbContext.Categories.FirstOrDefaultAsync(c => c.CategoryName == "Clothing"))?.Id
				},
				new Product
				{
					Id = 0,
					Name = "Smartphone",
					Description = "A high-end smartphone with advanced features and a sleek design",
					Price = 800,
					StockQuantity = 2,
					CategoryId = (await _appDbContext.Categories.FirstOrDefaultAsync(c => c.CategoryName == "Electronics"))?.Id
				},
				new Product
				{
					Id = 0,
					Name = "Backpack",
					Description = "A durable and spacious backpack for your travels",
					Price = 50,
					StockQuantity = 15,
					CategoryId = (await _appDbContext.Categories.FirstOrDefaultAsync(c => c.CategoryName == "Travel"))?.Id
				},
				new Product
				{
					Id = 0,
					Name = "Novel",
					Description = "A thrilling novel by your favorite author",
					Price = 20,
					StockQuantity = 20,
					CategoryId = (await _appDbContext.Categories.FirstOrDefaultAsync(c => c.CategoryName == "Books"))?.Id
				},
				new Product
				{
					Id = 0,
					Name = "Kitchen Appliances",
					Description = "A set of kitchen appliances for your modern kitchen",
					Price = 500,
					StockQuantity = 3,
					CategoryId = (await _appDbContext.Categories.FirstOrDefaultAsync(c => c.CategoryName == "Kitchen"))?.Id
				},
				// Add more products as needed
			};

			await _appDbContext.Products.AddRangeAsync(products);
			await _appDbContext.SaveChangesAsync();
		}
	}
}
