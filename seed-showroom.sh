#!sh

cd Server

CS="Server=localhost,1433;User Id=sa;Password=P@ssw0rd"

echo "Seeding databases"

echo "Seeding Showroom"
dotnet run --project ./Showroom/WebApi/WebApi.csproj -- --seed --connection-string "$CS;Database=Showroom"
echo "Done"

cd ..

echo "All done"