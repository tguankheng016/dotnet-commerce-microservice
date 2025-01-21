FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["services/shared/CommerceMicro.Modules.Core/CommerceMicro.Modules.Core.csproj", "shared/CommerceMicro.Modules.Core/"]
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
COPY ["services/cart/src/CommerceMicro.CartService.Application/CommerceMicro.CartService.Application.csproj", "cart/src/CommerceMicro.CartService.Application/"]
COPY ["services/cart/src/CommerceMicro.CartService.Application/Products/GrpcClient/Protos/", "cart/src/CommerceMicro.CartService.Application/Products/GrpcClient/Protos/"]
COPY ["services/cart/src/CommerceMicro.CartService.Api/CommerceMicro.CartService.Api.csproj", "cart/src/CommerceMicro.CartService.Api/"]
RUN dotnet restore "cart/src/CommerceMicro.CartService.Api/CommerceMicro.CartService.Api.csproj"
COPY ./services .
WORKDIR "/src/cart/src/CommerceMicro.CartService.Api"
RUN dotnet build "CommerceMicro.CartService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "CommerceMicro.CartService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CommerceMicro.CartService.Api.dll"]
