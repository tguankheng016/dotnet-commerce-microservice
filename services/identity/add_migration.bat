@echo off

:: Set the migration name
set MIGRATION_NAME=Initial

:: Set the migration project
set MIGRATION_PROJECT=src\CommerceMicro.IdentityService.Application\CommerceMicro.IdentityService.Application.csproj

:: Set the startup project
set STARTUP_PROJECT=src\CommerceMicro.IdentityService.Api\CommerceMicro.IdentityService.Api.csproj

:: Set the DbContext with namespace
set DBCONTEXT_WITH_NAMESPACE=CommerceMicro.IdentityService.Application.Data.AppDbContext

:: Set the output directory
set OUTPUT_DIR=Data\Migrations

:: Run the migration
dotnet ef migrations add --project %MIGRATION_PROJECT% --startup-project %STARTUP_PROJECT% --context %DBCONTEXT_WITH_NAMESPACE% --configuration Debug --verbose %MIGRATION_NAME% --output-dir %OUTPUT_DIR%

pause