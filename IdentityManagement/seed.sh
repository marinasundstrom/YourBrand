#!sh

source ../setup-dev.sh

echo "Seeding Users"
dotnet run --project ./Users/Users.csproj -- --seed --connection-string "$CS;Database=Users"
echo "Done"