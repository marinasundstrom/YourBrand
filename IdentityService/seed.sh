#!sh

source ../setup-dev.sh

echo "Seeding IdentityService"
dotnet run --project ./IdentityService/IdentityService.csproj -- --seed --connection-string "$CS;Database=IdentityServer"
echo "Done"