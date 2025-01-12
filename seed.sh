# Run this when AppHost is running. It will seed essential services.

dotnet run --project ./src/IdentityManagement/IdentityManagement/IdentityManagement.csproj -- --seed
dotnet run --project ./src/AppService/WebApi/WebApi.csproj -- --seed
dotnet run --project ./src/Notifications/Notifications/Notifications.csproj -- --seed