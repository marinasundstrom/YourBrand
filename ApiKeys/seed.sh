#!sh

source ../setup-dev.sh

echo "Seeding ApiKeys"
dotnet run --project ./ApiKeys/ApiKeys.csproj -- --seed --connection-string "$CS;Database=ApiKeys"
echo "Done"