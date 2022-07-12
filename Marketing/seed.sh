#!sh

source ../setup-dev.sh

echo "Seeding Marketing"
dotnet run --project ./Marketing/Marketing.csproj -- --seed --connection-string "$CS;Database=Marketing"
echo "Done"