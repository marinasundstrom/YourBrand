#!sh

source ../setup-dev.sh

echo "Seeding Showroom"
dotnet run --project ./WebApi/WebApi.csproj -- --seed --connection-string "$CS;Database=Showroom"
echo "Done"

if [[ $1 == "--sync-users" ]]; then
    cd ..; source sync-users.sh
fi