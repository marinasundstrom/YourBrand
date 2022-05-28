#!sh

CS="Server=localhost,1433;User Id=sa;Password=P@ssw0rd"

echo "Seeding databases"

echo "Seeding IdentityService"
dotnet run --project ./IdentityService/IdentityService/IdentityService.csproj -- --seed --connection-string "$CS;Database=IdentityServer"
echo "Done"

echo "Seeding Catalog"
dotnet run --project ./AppService/WebApi/WebApi.csproj -- --seed --connection-string "$CS;Database=Catalog"
echo "Done"

echo "Seeding TimeReport"
dotnet run --project ./TimeReport/WebApi/WebApi.csproj -- --seed --connection-string "$CS;Database=TimeReport"
echo "Done"

echo "Seeding Showroom"
dotnet run --project ./Showroom/WebApi/WebApi.csproj -- --seed --connection-string "$CS;Database=Showroom"
echo "Done"

echo "Seeding Worker"
dotnet run --project ./Worker/Worker/Worker.csproj -- --seed --connection-string "$CS;Database=Worker"
echo "Done"

echo "Seeding ApiKeys"
dotnet run --project ./ApiKeys/ApiKeys/ApiKeys.csproj -- --seed --connection-string "$CS;Database=ApiKeys"
echo "Done"

# Finance

echo "Seeding Accounting"
dotnet run --project ./Finance/Accounting/Accounting/WebAPI/WebApi.csproj -- --seed --connection-string "$CS;Database=Accounting"
echo "Done"

echo "Seeding Documents"
dotnet run --project ./Finance/Documents/Documents/Documents.csproj -- --seed --connection-string "$CS;Database=Documents"
echo "Done"

echo "Seeding Invoices"
dotnet run --project ./Finance/Invoices/Invoices/Invoices.csproj -- --seed --connection-string "$CS;Database=Incoices"
echo "Done"

echo "Seeding Payments"
dotnet run --project ./Finance/Payments/Payments/Payments.csproj -- --seed --connection-string "$CS;Database=Payments"
echo "Done"

echo "Seeding Transactions"
dotnet run --project ./Finance/Transactions/Transactions/Transactions.csproj -- --seed --connection-string "$CS;Database=Transactions"
echo "Done"

# End Finance

dotnet run --project Seeder/Seeder.csproj

echo "All done"