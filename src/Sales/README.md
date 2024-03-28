# Sales

## Migrations

Adding migration

```
dotnet ef migrations add <Name>
```

Update database

```
dotnet ef database update --connection "Server=localhost,1433;User Id=sa;Password=P@ssw0rd;Encrypt=false;Database=yourbrand-sales-db"
```

_* This is the connection string to "yourbrand-sales-db" running locally in Docker._

## Seeding test data

```
dotnet run --project Sales.API -- --seed
```

This will create a sample cart with id "test". It is currently being used during development.

### Seeding remote database (such as in Azure SQL database)

In ``launchSettings.json`` , temporarily change the value of key ``ConnectionStrings__SalesDb`` to desired connection string - like the remote SQL Server. _(Default is local)_

Then execute:

```
dotnet run --project Sales.API -- --seed
```

Revert the changes to restore connection string to local database.
