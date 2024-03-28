FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

USER app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /
COPY ["src/StoreFront/StoreFront.API/StoreFront.API.csproj", "src/StoreFront/StoreFront.API/"]
COPY ["NuGet.config", "/"]
RUN dotnet restore "src/StoreFront/StoreFront.API/StoreFront.API.csproj"
COPY . .
WORKDIR "/src/StoreFront/StoreFront.API"
RUN dotnet build "StoreFront.API.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "StoreFront.API.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StoreFront.API.dll"]
