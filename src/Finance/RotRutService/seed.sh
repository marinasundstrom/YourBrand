#!sh

source ../../setup-dev.sh

echo "Seeding RotRutService"
dotnet run --project ./RotRutService/RotRutService.csproj -- --seed --connection-string "$CS;Database=RotRutService"
echo "Done"