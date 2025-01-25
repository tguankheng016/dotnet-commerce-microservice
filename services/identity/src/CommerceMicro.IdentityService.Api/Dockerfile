FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["services/shared/CommerceMicro.Modules.Core/CommerceMicro.Modules.Core.csproj", "shared/CommerceMicro.Modules.Core/"]
COPY ["services/shared/CommerceMicro.Modules.Azure/CommerceMicro.Modules.Azure.csproj", "shared/CommerceMicro.Modules.Azure/"]
COPY ["services/shared/CommerceMicro.Modules.Caching/CommerceMicro.Modules.Caching.csproj", "shared/CommerceMicro.Modules.Caching/"]
COPY ["services/shared/CommerceMicro.Modules.Contracts/CommerceMicro.Modules.Contracts.csproj", "shared/CommerceMicro.Modules.Contracts/"]
COPY ["services/shared/CommerceMicro.Modules.Logging/CommerceMicro.Modules.Logging.csproj", "shared/CommerceMicro.Modules.Logging/"]
COPY ["services/shared/CommerceMicro.Modules.MassTransit/CommerceMicro.Modules.MassTransit.csproj", "shared/CommerceMicro.Modules.MassTransit/"]
COPY ["services/shared/CommerceMicro.Modules.OpenTelemetry/CommerceMicro.Modules.OpenTelemetry.csproj", "shared/CommerceMicro.Modules.OpenTelemetry/"]
COPY ["services/shared/CommerceMicro.Modules.Permissions/CommerceMicro.Modules.Permissions.csproj", "shared/CommerceMicro.Modules.Permissions/"]
COPY ["services/shared/CommerceMicro.Modules.Permissions/GrpcClient/", "shared/CommerceMicro.Modules.Permissions/GrpcClient/"]
COPY ["services/shared/CommerceMicro.Modules.Postgres/CommerceMicro.Modules.Postgres.csproj", "shared/CommerceMicro.Modules.Postgres/"]
COPY ["services/shared/CommerceMicro.Modules.Resiliency/CommerceMicro.Modules.Resiliency.csproj", "shared/CommerceMicro.Modules.Resiliency/"]
COPY ["services/shared/CommerceMicro.Modules.Security/CommerceMicro.Modules.Security.csproj", "shared/CommerceMicro.Modules.Security/"]
COPY ["services/shared/CommerceMicro.Modules.Security/GrpcClient/", "shared/CommerceMicro.Modules.Security/GrpcClient/"]
COPY ["services/shared/CommerceMicro.Modules.Web/CommerceMicro.Modules.Web.csproj", "shared/CommerceMicro.Modules.Web/"]
COPY ["services/identity/src/CommerceMicro.IdentityService.Application/CommerceMicro.IdentityService.Application.csproj", "identity/src/CommerceMicro.IdentityService.Application/"]
COPY ["services/identity/src/CommerceMicro.IdentityService.Application/Identities/GrpcServer/Protos/", "identity/src/CommerceMicro.IdentityService.Application/Identities/GrpcServer/Protos/"]
COPY ["services/identity/src/CommerceMicro.IdentityService.Api/CommerceMicro.IdentityService.Api.csproj", "identity/src/CommerceMicro.IdentityService.Api/"]
RUN dotnet restore "identity/src/CommerceMicro.IdentityService.Api/CommerceMicro.IdentityService.Api.csproj"
COPY ./services .
WORKDIR "/src/identity/src/CommerceMicro.IdentityService.Api"
RUN dotnet build "CommerceMicro.IdentityService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "CommerceMicro.IdentityService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CommerceMicro.IdentityService.Api.dll"]
