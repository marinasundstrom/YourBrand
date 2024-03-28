#!sh

source ../setup-dev.sh

echo "Seeding ApiKeys"
dotnet run --project ./ApiKeys/ApiKeys.csproj -- --seed --connection-string "$CS;Database=ApiKeys"
echo "Done"

if [[ $1 == "--sync-users" ]]; then
    cd ..; source sync-users.sh
fi