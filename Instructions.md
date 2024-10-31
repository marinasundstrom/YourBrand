# Instructions

1. Run services in Aspire (Requires .NET 9 SDK and Docker):

``dotnet run --project src/YourBrand.AppHost/YourBrand.AppHost.csproj;``

2. Run ``seed.sh`` for seeding initial services databases. This will create default data with test tenant, users, and organisations .

3. Login at ``https://localhost:5040/``, as username ``alice`` (or ``bob``) and password ``Pass123$``. Since the portal will not prompt you to login. (Needs fix)

4. Go to the Portal: https://localhost:5174/ 

5. Activate the desired Portal modules at ``Menu > Administration > Modules``. Click "Populate modules" and "Reload app". (Don't click "Populate" more than once!)

6. Seed the other services as you need.

Activate Portal module “Identity Management” for this:

7. Go ``Menu > Administration > Sync`` to sync and re-sync user data across services. Also used after recreating and seeding service databases.

Keep in mind that modules depend on other modules. ``Sales`` depend on ``Customers``. So activate both.