#!sh

source ../../setup-dev.sh

echo "Seeding Accounting"
dotnet run --project ./Accounting/WebApi/WebApi.csproj -- --seed --connection-string "$CS;Database=Accounting"
echo "Done"