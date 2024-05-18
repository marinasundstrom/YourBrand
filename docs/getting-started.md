# Getting started

* Aspire Dashboard: https://localhost:17125/
* Portal: https://localhost:5174/ (Actually the reverse proxy)
* Identity Management: https://localhost:5040/
* Store: https://localhost:7188/

## Run with Aspire

```sh
dotnet run --project src/YourBrand.AppHost/YourBrand.AppHost.csproj
```

### Important service

These services are essential for the function of YourBrand:

* Proxy
* Portal
* AppService
* IdentityManagement
* HumanResources
* ApiKeys

### Syncing user data

Everytime a database is created and recreated, you must populate it with users.

Initial creation from seed:

```
dotnet run --project Seeder/Seeder.csproj -- --seed
```

To sync users to services (will make sure users have been created):

```
dotnet run --project Seeder/Seeder.csproj -- --sync
```

The services must be running.

A ``CreateUser`` message will be published, and each service will consume that message, creating a local user if not already existing.

## Setting up a company

You have to create a company in the Portal, in ``Administration > Set up``.

Default credentials:

```
AliceSmith@email.com
Pass123$
```

These credentials are used when logging in for the first time.

## DevTunnel

To connect from remote:

```
devtunnel host -p 5174 -a
```