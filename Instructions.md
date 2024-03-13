# Instructions

Nginx has been replaced by YARP - the Proxy project.

* Portal: https://localhost:5174/ (Actually the reverse proxy)
* Identity server: https://localhost:5040/

## Seeding databases

You need to seed the database for each service.

```
dotnet run -- --seed
```

## Running services

Just run each service:

```
dotnet run
```

If you use VS Code, and you have the Restore Terminals extension installed, there will be a terminal for each service.

### Important service

These services are essential for the function of YourBrand:

* ApiKeys
* IdentityService
* AppService
* HumanResources

### Syncing user data

Everytime a database is created and recerated, you must populate it with users.

```
dotnet run --project Seeder/Seeder.csproj -- --sync-users
```

The services must be running.

A ``CreateUser`` message will be published, and each service will consume that message, creating a local user if not already existing.

## Setting up a company

You have to create a company in the Portal, in ``Administration > Set up``.

Default credentials:

```
admin@email.com
Abc123!?
```

These credentials are used when logging in for the first time.

## DevTunnel

To connect from remote:

```
devtunnel host -p 5174 -a
```