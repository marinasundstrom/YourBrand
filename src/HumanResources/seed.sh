#!sh

source ../setup-dev.sh

echo "Seeding HumanResources"
dotnet run --project ./HumanResources/HumanResources.csproj -- --seed --connection-string "$CS;Database=HumanResources"
echo "Done"