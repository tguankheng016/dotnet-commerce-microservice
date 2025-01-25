@echo off

:: Set the migration name
set MIGRATION_NAME=Added_Product

:: Set the migration project
set MIGRATION_PROJECT=src\CommerceMicro.ProductService.Application\CommerceMicro.ProductService.Application.csproj

:: Set the startup project
set STARTUP_PROJECT=src\CommerceMicro.ProductService.Api\CommerceMicro.ProductService.Api.csproj

:: Set the DbContext with namespace
set DBCONTEXT_WITH_NAMESPACE=CommerceMicro.ProductService.Application.Data.AppDbContext

:: Set the output directory
set OUTPUT_DIR=Data\Migrations

:: Run the migration
dotnet ef migrations add --project %MIGRATION_PROJECT% --startup-project %STARTUP_PROJECT% --context %DBCONTEXT_WITH_NAMESPACE% --configuration Debug --verbose %MIGRATION_NAME% --output-dir %OUTPUT_DIR%

pause