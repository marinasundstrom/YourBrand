#!sh

source ../../setup-dev.sh

echo "Seeding Payments"
dotnet run --project ./Payments/Payments.csproj -- --seed --connection-string "$CS;Database=Payments"
echo "Done"