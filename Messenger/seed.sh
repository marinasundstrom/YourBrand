#!sh

source ../setup-dev.sh

echo "Seeding Messenger"
dotnet run --project ./Messenger/Messenger.csproj -- --seed --connection-string "$CS;Database=Messenger"
echo "Done"

if [[ $1 == "--sync-users" ]]; then
    cd ..; source sync-users.sh
fi