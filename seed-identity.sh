#!sh

cd Server

CS="Server=localhost,1433;User Id=sa;Password=P@ssw0rd"

echo "Seeding databases"

echo "Seeding IdentityService"
dotnet run --project ./IdentityService/IdentityService/IdentityService.csproj -- --seed --connection-string "$CS;Database=IdentityServer"
echo "Done"

cd ..

echo "All done"