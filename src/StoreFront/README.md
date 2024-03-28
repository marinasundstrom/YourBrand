# StoreFront

This service provides an API that sales sites integrate with. In that way it acts as an API gateway.

## Seeding test data

```
dotnet run --project StoreFront.API -- --seed
```

This will create a sample cart with id "test". It is currently being used during development.

### Seeding remote database (such as in Azure SQL database)

In ``launchSettings.json`` , temporarily change the value of key ``ConnectionStrings__StoreFrontDb`` to desired connection string - like the remote SQL Server. _(Default is local)_

Then execute:

```
dotnet run --project StoreFront.API -- --seed
```

Revert the changes to restore connection string to local database.
