# Sync data

Services usually rely on locally cached tenant and user (identity) data to work. 

This is automatically updated using event notifications originating from the `IdentityManagement` services. So each time a tenant or user is created or updated the service will publish a notification that service may subscribe to.

You can trigger synchronization of data from the Portal. In order to do so, activate Portal module “Identity Management” for this:

Then, go ``Menu > Administration > Sync`` to sync and re-sync user data across services. 

This is particulary useful during development after recreating and seeding service databases.

Alternatively, you can sync using the `Seeder` project: `dotnet run -- --sync``