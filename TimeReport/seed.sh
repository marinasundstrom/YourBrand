#!sh

source ../setup-dev.sh

echo "Seeding TimeReport"
dotnet run --project ./WebApi/WebApi.csproj -- --seed --connection-string "$CS;Database=TimeReport"
echo "Done"