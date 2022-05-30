#!sh

source ../setup-dev.sh

echo "Seeding Customers"
dotnet run --project ./Customers/Customers.csproj -- --seed --connection-string "$CS;Database=Customers"
echo "Done"