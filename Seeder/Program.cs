using Microsoft.Extensions.DependencyInjection;

using YourBrand.TimeReport;
using YourBrand.HumanResources;
using YourBrand.HumanResources.Client;

const string ApiKey = "asdsr34#34rswert35234aedae?2!";

var services = BuildServiceProvider();

var organizationsClient = services.GetRequiredService<IOrganizationsClient>();
var personsClient = services.GetRequiredService<IPersonsClient>();
var syncClient = services.GetRequiredService<ISyncClient>();

if (args.ToArray().Contains("--sync-users"))
{
    await syncClient.SyncDataAsync();
    return;
}

Console.WriteLine("Creating organization...");

var org = await organizationsClient.CreateOrganizationAsync(new CreateOrganizationDto
{
    Name = "My org",
    FriendlyName = "my-org",
});

Console.WriteLine("Creating users...");

var userAdmin = await personsClient.CreatePersonAsync(new CreatePersonDto
{
    FirstName = "Administrator",
    LastName = "Administrator",
    Title = "Administrator",
    DisplayName = "Administrator",
    Role = "Administrator",
    Ssn = "",
    Email = "admin@email.com",
    Password = "Abc123!?",
    OrganizationId = org.Id
});

var userTest = await personsClient.CreatePersonAsync(new CreatePersonDto
{
    FirstName = "Test",
    LastName = "Testsson",
    Title = "Software Developer",
    Role = "User",
    Ssn = "",
    Email = "test@email.com",
    Password = "Abc123!?",
    OrganizationId = org.Id
});

Console.WriteLine("Users created");

if (args.ToArray().Contains("--create-projects"))
{
    Console.WriteLine("Creating projects...");

    var organizationClient = services.GetRequiredService<YourBrand.TimeReport.Client.IOrganizationsClient>();
    var projectsClient = services.GetRequiredService<YourBrand.TimeReport.Client.IProjectsClient>();
    var activitiesClient = services.GetRequiredService<YourBrand.TimeReport.Client.IActivitiesClient>();
    var activityTypesClient = services.GetRequiredService<YourBrand.TimeReport.Client.IActivityTypesClient>();

    var organization = await organizationClient.CreateOrganizationAsync(new YourBrand.TimeReport.Client.CreateOrganizationDto()
    {
        Name = "ACME Inc."
    });

    var workActivityType = await activityTypesClient.CreateActivityTypeAsync(new YourBrand.TimeReport.Client.CreateActivityTypeDto()
    {
        Name = "Chargeable",
        ExcludeHours = false,
        OrganizationId = organization.Id
    });

    var absenceActivityType = await activityTypesClient.CreateActivityTypeAsync(new YourBrand.TimeReport.Client.CreateActivityTypeDto()
    {
        Name = "Absence",
        ExcludeHours = true,
        OrganizationId = organization.Id
    });

    var projectMyProject = await projectsClient.CreateProjectAsync(new YourBrand.TimeReport.Client.CreateProjectDto()
    {
        Name = "My mega project",
        OrganizationId = organization.Id
    });

    var activityWork = await activitiesClient.CreateActivityAsync(projectMyProject.Id, new YourBrand.TimeReport.Client.CreateActivityDto()
    {
        Name = "Konsultarbete",
        ActivityTypeId = workActivityType.Id,
        HourlyRate = 890
    });

    var activityMisc = await activitiesClient.CreateActivityAsync(projectMyProject.Id, new YourBrand.TimeReport.Client.CreateActivityDto()
    {
        Name = "Misc",
        ActivityTypeId = workActivityType.Id
    });

    await projectsClient.CreateProjectMembershipAsync(projectMyProject.Id, new YourBrand.TimeReport.Client.CreateProjectMembershipDto
    {
        UserId = userTest.Id
    });

    var projectInternal = await projectsClient.CreateProjectAsync(new YourBrand.TimeReport.Client.CreateProjectDto()
    {
        Name = "Internal",
        OrganizationId = organization.Id
    });

    var activitySick = await activitiesClient.CreateActivityAsync(projectInternal.Id, new YourBrand.TimeReport.Client.CreateActivityDto()
    {
        Name = "Sick",
        ActivityTypeId = absenceActivityType.Id
    });

    await projectsClient.CreateProjectMembershipAsync(projectInternal.Id, new YourBrand.TimeReport.Client.CreateProjectMembershipDto
    {
        UserId = userAdmin.Id
    });

    await projectsClient.CreateProjectMembershipAsync(projectInternal.Id, new YourBrand.TimeReport.Client.CreateProjectMembershipDto
    {
        UserId = userTest.Id
    });

    Console.WriteLine("Projects created");
}

static IServiceProvider BuildServiceProvider()
{
    ServiceCollection services = new();

    services.AddHumanResourcesClients((sp, http) =>
    {
        http.BaseAddress = new Uri($"https://localhost/api/humanresources/");
        //http.DefaultRequestHeaders.Add("X-API-KEY", ApiKey);
    }, (builder) => { });

    services.AddTimeReportClients((sp, http) =>
    {
        http.BaseAddress = new Uri($"https://localhost/api/timereport/");
        http.DefaultRequestHeaders.Add("X-API-KEY", ApiKey);
    }, (builder) => { });

    return services.BuildServiceProvider();
}