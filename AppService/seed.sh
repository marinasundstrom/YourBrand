#!sh

source ../setup-dev.sh

echo "Seeding AppService"
dotnet run --project ./WebApi/WebApi.csproj -- --seed --connection-string "$CS;Database=AppService"
echo "Done"