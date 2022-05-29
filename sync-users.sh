#!sh

echo "Synching users"

dotnet run --project Seeder/Seeder.csproj -- --sync-users