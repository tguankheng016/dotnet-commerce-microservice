# Add Migrations
MIGRATION_NAME = Added_UserRolePermissions
MIGRATION_PROJECT = src/CommerceMicro.CartService.Application/CommerceMicro.CartService.Application.csproj
STARTUP_PROJECT = src/CommerceMicro.CartService.Api/CommerceMicro.CartService.Api.csproj
DBCONTEXT_WITH_NAMESPACE = CommerceMicro.CartService.Application.Data.AppDbContext
OUTPUT_DIR = Data/Migrations
add_migration:
	dotnet ef migrations add --project $(MIGRATION_PROJECT) --startup-project $(STARTUP_PROJECT) --context $(DBCONTEXT_WITH_NAMESPACE) --configuration Debug --verbose $(MIGRATION_NAME) --output-dir $(OUTPUT_DIR)

run:
	dotnet run --project $(STARTUP_PROJECT)