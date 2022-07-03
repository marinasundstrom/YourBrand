#!sh

source ../setup-dev.sh

echo "Seeding Products"
dotnet run --project ./Products/Products.csproj -- --seed --connection-string "$CS;Database=Products"
echo "Done"