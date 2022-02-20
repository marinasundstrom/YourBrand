#!sh

cd Server

dotnet run ./IdentityService/IdentityService.csproj -- --seed
dotnet run ./AppService/WebApi/WebApi.csproj -- --seed
dotnet run ./TimeReport/WebApi/WebApi.csproj -- --seed
dotnet run ./Worker/Worker.csproj -- --seed
