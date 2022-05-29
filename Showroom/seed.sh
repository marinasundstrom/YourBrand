#!sh

source ../setup-dev.sh

echo "Seeding Showroom"
dotnet run --project ./WebApi/WebApi.csproj -- --seed --connection-string "$CS;Database=Showroom"
echo "Done"