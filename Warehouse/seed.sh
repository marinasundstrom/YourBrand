#!sh

source ../setup-dev.sh

echo "Seeding Warehouse"
dotnet run --project ./Warehouse/Warehouse.csproj -- --seed --connection-string "$CS;Database=Warehouse"
echo "Done"