#!sh

source ../../setup-dev.sh

echo "Seeding Invoices"
dotnet run --project ./Invoices/Invoices.csproj -- --seed --connection-string "$CS;Database=Invoices"
echo "Done"