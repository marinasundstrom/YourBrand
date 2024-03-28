#!sh

source ../setup-dev.sh

echo "Seeding Notifications"
dotnet run --project ./Notifications/Notifications.csproj -- --seed --connection-string "$CS;Database=Notifications"
echo "Done"