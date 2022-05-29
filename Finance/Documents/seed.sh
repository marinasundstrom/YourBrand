#!sh

source ../../setup-dev.sh

echo "Seeding Documents"
dotnet run --project ./Documents/Documents.csproj -- --seed --connection-string "$CS;Database=Documents"
echo "Done"