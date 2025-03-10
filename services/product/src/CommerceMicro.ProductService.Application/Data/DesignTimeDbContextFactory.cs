using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CommerceMicro.ProductService.Application.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<AppDbContext>();

        builder.UseNpgsql("Server=localhost;Port=5433;Database=CommerceMicroDb;User Id=postgres;Password=myStong_Password123#;Include Error Detail=true")
            .UseSnakeCaseNamingConvention();

        return new AppDbContext(builder.Options);
    }
}
