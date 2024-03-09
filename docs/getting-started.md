This will be updated: Tye is not used anymore.

# Getting Started

This guide is intended to be run from top-down.

All the necessary services have been configured in the ```docker-compose.deps.yml``` file.

## Prerequisites

### Install the .NET 8 SDK

Download and run the [installer](https://dotnet.microsoft.com/en-us/download/dotnet/8.0).

### Install Docker Desktop

Download it from [here](https://www.docker.com/products/docker-desktop).

### Run and seed projects

Each project has to be seeded  

## Run the Docker services

Open a terminal and navigate to the root/solution folder. 

Run the following command:

```sh
docker-compose up docker-compose.deps.yml
```


## Create the app databases

This will create and seed the main app database.

In the source file ```Server/AppService/WebApi/Program.cs```, scroll down to the following line:

```C#
//await app.Services.SeedAsync();
```

Toggle the comment off.

```C#
await app.Services.SeedAsync();
```

If you are in Watch mode, then the service will restart, and the uncommented code will execute.

Make sure to toggle the comment on again, or else the database will be recreated everytime you make a change to AppService.

*This line of code has to be toggled off everytime the Domain model change in order for the database to be recreated.*

## Seed IdentityService database

The following steps will seed the database.

In a terminal, go to the ```Server/IdentityService``` project directory.

Make sure that the database server is running. *(See previous steps)*

Run this command:

```sh
dotnet run -- /seed
```

## Set up Azurite Storage Emulator

To publicly expose Blobs via their URLs you have to change Azurite's configuration.

*(This requires Azurite to have been run once for the files to be created)*

Open the file ```WebApi/.data/azurite/__azurite_db_blob__.json```:

Add the ```"publicAccess": "blob"``` key-value in the section shown below:

```json
        {
            "name": "$CONTAINERS_COLLECTION$",
            "data": [
                {
                    "accountName": "devstoreaccount1",
                    "name": "images",
                    "properties": {
                        "etag": "\"0x1C839AE6CDF11F0\"",
                        "lastModified": "2021-05-14T15:08:51.726Z",
                        "leaseStatus": "unlocked",
                        "leaseState": "available",
                        "hasImmutabilityPolicy": false,
                        "hasLegalHold": false,
              --- >  "publicAccess": "blob" <---- 
                    },
                   // Omitted
        },
```

Then, restart Azurite. 

Just restart the whole system. Exit Tye, and restart it.

## Launch the web app

In your browser, navigate to: ```https://localhost/```

You are now ready.
