using CommerceMicro.ProductService.Application.Data;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace CommerceMicro.ProductService.Application.Products.GrpcServers.Services;

public class ProductGrpcServices : ProductGrpcService.ProductGrpcServiceBase
{
	private readonly AppDbContext _appDbContext;

	public ProductGrpcServices(
		AppDbContext appDbContext
	)
	{
		_appDbContext = appDbContext;
	}

	public override async Task<GetAllProductsResponse> GetAllProducts(GetAllProductsRequest request, ServerCallContext context)
	{
		var products = await _appDbContext.Products.ToListAsync();

		var response = new GetAllProductsResponse();

		foreach (var product in products)
		{
			response.Products.Add(new ProductModel()
			{
				Id = product.Id,
				Name = product.Name,
				Description = product.Description,
				StockQuantity = product.StockQuantity,
				Price = (double)product.Price,
			});
		}

		return response;
	}
}
