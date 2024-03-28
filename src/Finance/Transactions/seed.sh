#!sh

source ../../setup-dev.sh

echo "Seeding Transactions"
dotnet run --project ./Transactions/Transactions.csproj -- --seed --connection-string "$CS;Database=Transactions"
echo "Done"