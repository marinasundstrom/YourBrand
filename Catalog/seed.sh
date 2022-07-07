#!sh

source ../setup-dev.sh

echo "Seeding Catalog"
dotnet run --project ./Catalog/Catalog.csproj -- --seed --connection-string "$CS;Database=Catalog"
echo "Done"