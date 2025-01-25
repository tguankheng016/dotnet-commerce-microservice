FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["gateways/web/CommerceMicro.Gateways.Web/CommerceMicro.Gateways.Web.csproj", "gateways/web/CommerceMicro.Gateways.Web/"]
RUN dotnet restore "gateways/web/CommerceMicro.Gateways.Web/CommerceMicro.Gateways.Web.csproj"
COPY ./gateways ./gateways
WORKDIR "/src/gateways/web/CommerceMicro.Gateways.Web"
RUN dotnet build "CommerceMicro.Gateways.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "CommerceMicro.Gateways.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CommerceMicro.Gateways.Web.dll"]
