#!sh

source ../setup-dev.sh

echo "Seeding Orders"
dotnet run --project ./Orders/Orders.csproj -- --seed --connection-string "$CS;Database=Orders"
echo "Done"