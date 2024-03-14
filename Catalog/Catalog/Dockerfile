FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

USER app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /
COPY ["src/Catalog/Catalog.API/Catalog.csproj", "src/Catalog/Catalog.API/"]
COPY ["NuGet.config", "/"]
RUN dotnet restore "src/Catalog/Catalog.API/Catalog.csproj"
COPY . .
WORKDIR "/src/Catalog/Catalog.API"
RUN dotnet build "Catalog.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "Catalog.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Catalog.dll"]
