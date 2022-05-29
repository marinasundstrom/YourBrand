#!sh

source ../setup-dev.sh

echo "Seeding TimeReport"
dotnet run --project ./WebApi/WebApi.csproj -- --seed --connection-string "$CS;Database=TimeReport"
echo "Done"

if [[ $1 == "--sync-users" ]]; then
    cd ..; source sync-users.sh
fi