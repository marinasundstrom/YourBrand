#!sh

source ../setup-dev.sh

echo "Seeding Inventory"
dotnet run --project ./Inventory/Inventory.csproj -- --seed --connection-string "$CS;Database=Inventory"
echo "Done"