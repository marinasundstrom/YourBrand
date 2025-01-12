﻿using Microsoft.Extensions.DependencyInjection;

using YourBrand.IdentityManagement.Client;

//const string ApiKey = "asdsr34#34rswert35234aedae?2!";

var services = BuildServiceProvider();

if (args.ToArray().Contains("--sync"))
{
    var syncClient = services.GetRequiredService<ISyncClient>();

    await syncClient.SyncDataAsync();
    return;
}

if (args.ToArray().Contains("--seed"))
{
    var organizationsClient = services.GetRequiredService<IOrganizationsClient>();
    var usersClient = services.GetRequiredService<IUsersClient>();

    var organization = organizationsClient.CreateOrganizationAsync(new CreateOrganization
    {
        Name = "ACME Inc.",
        FriendlyName = "ACME Inc."
    });

    var alice = usersClient.CreateUserAsync(new CreateUser
    {
        FirstName = "Administrator",
        LastName = "Administrator",
        DisplayName = "Administrator",
        Email = "admin@email.com"
    });

    /*
        var alice = usersClient.CreateUserAsync(new CreateUser
        {
            FirstName = "Alice",
            LastName = "Smith",
            Email = "AliceSmith@email.com"
        });

        var bob = usersClient.CreateUserAsync(new CreateUser
        {
            FirstName = "Bob",
            LastName = "Smith",
            Email = "BobSmith@email.com"
        });
    */
}

static IServiceProvider BuildServiceProvider()
{
    ServiceCollection services = new();

    services.AddIdentityManagementClients((sp, http) =>
    {
        http.BaseAddress = new Uri($"https://localhost:5174/api/identitymanagement/");
        //http.DefaultRequestHeaders.Add("X-API-Key", ApiKey);
    }, (builder) => { });

    return services.BuildServiceProvider();
}