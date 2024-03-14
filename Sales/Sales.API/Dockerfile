FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

USER app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /
COPY ["src/Sales/Sales.API/Sales.API.csproj", "src/Sales/Sales.API/"]
COPY ["NuGet.config", "/"]
RUN dotnet restore "src/Sales/Sales.API/Sales.API.csproj"
COPY . .
WORKDIR "/src/Sales/Sales.API"
RUN dotnet build "Sales.API.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "Sales.API.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sales.API.dll"]
