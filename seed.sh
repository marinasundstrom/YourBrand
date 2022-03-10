#!sh

cd Server

CS="Server=localhost,1433;User Id=sa;Password=P@ssw0rd"

echo "Seeding databases"

echo "Seeding IdentityService"
dotnet run --project ./IdentityService/IdentityService.csproj -- --seed --connection-string "$CS;Database=IdentityServer"
echo "Done"

echo "Seeding Catalog"
dotnet run --project ./AppService/WebApi/WebApi.csproj -- --seed --connection-string "$CS;Database=Catalog"
echo "Done"

echo "Seeding TimeReport"
dotnet run --project ./TimeReport/WebApi/WebApi.csproj -- --seed --connection-string "$CS;Database=TimeReport"
echo "Done"

echo "Seeding Showroom"
dotnet run --project ./Showroom/WebApi/WebApi.csproj -- --seed --connection-string "$CS;Database=Showroom"
echo "Done"

echo "Seeding Worker"
dotnet run --project ./Worker/Worker.csproj -- --seed --connection-string "$CS;Database=Worker"
echo "Done"

cd ..

echo "All done"