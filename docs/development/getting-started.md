# Getting started

1. Run services in Aspire (Requires .NET 10 SDK and Docker):

``dotnet run --project src/YourBrand.AppHost/YourBrand.AppHost.csproj;``

2. Run ``seed.sh`` for seeding the essential services' databases. This will create default data with test tenant, users, and organisations.

3. Go to the Portal: https://localhost:5174/ 

4. Click the "Login" button in the top menu, Enter username ``alice`` (or ``bob``) and password ``Pass123$``. Succeed with logging in.

Now you are in!

5. Activate the desired Portal modules at ``Menu > Administration > Modules``. Click "Populate modules" and "Reload app". (Don't click "Populate" more than once!)

Keep in mind that modules depend on other modules. ``Sales`` depend on ``Customers``. So activate both.

6. Seed the other services as you need.

Activate Portal module “Identity Management” for this:

7. Go ``Menu > Administration > Sync`` to sync and re-sync user data across services. Also used after recreating and seeding service databases.

Alternatively, you can sync using the `Seeder` project: `dotnet run -- --sync``

Seed the databases for the rest of the services as you need them, and when the Aspire app host is running:

```
dotnet run -- --seed
```
