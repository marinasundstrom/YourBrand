#!sh

source setup-dev.sh

echo "-- Seed Script --"

echo "WARNING: Both services and databases must have been started in order for this script to run as intended."

echo "Seeding databases"

cd ./IdentityService; sh seed.sh; cd ..;
cd ./AppService; sh seed.sh; cd ..;
cd ./TimeReport; sh seed.sh; cd ..;
cd ./Showroom; sh seed.sh; cd ..;
cd ./Worker; sh seed.sh; cd ..;
cd ./ApiKeys; sh seed.sh; cd ..;

# Finance

cd ./Finance/Accounting; sh seed.sh; cd ../..;
cd ./Finance/Documents; sh seed.sh; cd ../..;
cd ./Finance/Invoices; sh seed.sh; cd ../..;
cd ./Finance/Payments; sh seed.sh; cd ../..;
cd ./Finance/Transactions; sh seed.sh; cd ../..;

# End Finance

dotnet run --project Seeder/Seeder.csproj

echo "All done"