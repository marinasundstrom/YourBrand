#!sh

source ../setup-dev.sh

cd ./Accounting; sh seed.sh; cd ..;
cd ./Invoices; sh seed.sh; cd ..;
cd ./Payments; sh seed.sh; cd ..;
cd ./Transactions; sh seed.sh; cd ..;
cd ./RotRutService; sh seed.sh; cd ..;