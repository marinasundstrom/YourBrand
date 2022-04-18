using Microsoft.Extensions.DependencyInjection;
using YourBrand.IdentityService.Client;

const string ApiKey = "asdsr34#34rswert35234aedae?2!";

var services = BuildServiceProvider();

var usersClient = services.GetRequiredService<IUsersClient>();

Console.WriteLine("Creating users...");

var userAdmin = await usersClient.CreateUserAsync(new CreateUserDto
{
    FirstName = "Administrator",
    LastName = "Administrator",
    DisplayName = "Administrator",
    Role = "Administrator",
    Ssn = "",
    Email = "admin@email.com",
    Password = "Abc123!?"
});

var userTest = await usersClient.CreateUserAsync(new CreateUserDto
{
    FirstName = "Test",
    LastName = "Testsson",
    Role = "User",
    Ssn = "",
    Email = "test@email.com",
    Password = "Abc123!?"
});

Console.WriteLine("Users created");

Console.WriteLine("Creating projects...");

var projectsClient = services.GetRequiredService<YourBrand.TimeReport.Client.IProjectsClient>();
var activitiesClient = services.GetRequiredService<YourBrand.TimeReport.Client.IActivitiesClient>();

var projectMyProject = await projectsClient.CreateProjectAsync(new YourBrand.TimeReport.Client.CreateProjectDto() {
    Name = "My project"
});

var activityWork = await activitiesClient.CreateActivityAsync(projectMyProject.Id, new YourBrand.TimeReport.Client.CreateActivityDto() {
    Name = "Work"
});

var activityMisc = await activitiesClient.CreateActivityAsync(projectMyProject.Id, new YourBrand.TimeReport.Client.CreateActivityDto() {
    Name = "Misc"
});

await projectsClient.CreateProjectMembershipAsync(projectMyProject.Id, new YourBrand.TimeReport.Client.CreateProjectMembershipDto {
    UserId = userTest.Id
});

var projectInternal = await projectsClient.CreateProjectAsync(new YourBrand.TimeReport.Client.CreateProjectDto() {
    Name = "Internal"
});

var activitySick = await activitiesClient.CreateActivityAsync(projectInternal.Id, new YourBrand.TimeReport.Client.CreateActivityDto() {
    Name = "Sick"
});

await projectsClient.CreateProjectMembershipAsync(projectInternal.Id, new YourBrand.TimeReport.Client.CreateProjectMembershipDto {
    UserId = userAdmin.Id
});

await projectsClient.CreateProjectMembershipAsync(projectInternal.Id, new YourBrand.TimeReport.Client.CreateProjectMembershipDto {
    UserId = userTest.Id
});

Console.WriteLine("Projects created");

static IServiceProvider BuildServiceProvider()
{
    ServiceCollection services = new();

    services.AddHttpClient(nameof(IUsersClient), (sp, http) =>
    {
        http.BaseAddress = new Uri($"https://identity.local/");
        http.DefaultRequestHeaders.Add("X-API-KEY", ApiKey);
    })
    .AddTypedClient<IUsersClient>((http, sp) => new UsersClient(http));

    services.AddHttpClient(nameof(YourBrand.TimeReport.Client.IProjectsClient), (sp, http) =>
    {
        http.BaseAddress = new Uri($"https://localhost/timereport/");
        http.DefaultRequestHeaders.Add("X-API-KEY", ApiKey);
    })
    .AddTypedClient<YourBrand.TimeReport.Client.IProjectsClient>((http, sp) => new YourBrand.TimeReport.Client.ProjectsClient(http));

    services.AddHttpClient(nameof(YourBrand.TimeReport.Client.IActivitiesClient), (sp, http) =>
    {
        http.BaseAddress = new Uri($"https://localhost/timereport/");
        http.DefaultRequestHeaders.Add("X-API-KEY", ApiKey);
    })
    .AddTypedClient<YourBrand.TimeReport.Client.IActivitiesClient>((http, sp) => new YourBrand.TimeReport.Client.ActivitiesClient(http));

    return services.BuildServiceProvider();
}