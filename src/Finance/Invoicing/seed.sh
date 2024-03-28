#!sh

source ../../setup-dev.sh

echo "Seeding Invoicing"
dotnet run --project ./Invoicing/Invoicing.csproj -- --seed --connection-string "$CS;Database=Invoicing"
echo "Done"