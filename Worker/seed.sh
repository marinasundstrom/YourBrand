#!sh

source ../setup-dev.sh

echo "Seeding Worker"
dotnet run --project ./Worker/Worker.csproj -- --seed --connection-string "$CS;Database=Worker"
echo "Done"