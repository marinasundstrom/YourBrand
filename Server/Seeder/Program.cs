using Microsoft.Extensions.DependencyInjection;
using Skynet.IdentityService.Client;

const string ApiKey = "foobar";

var services = BuildServiceProvider();

var usersClient = services.GetRequiredService<IUsersClient>();

await usersClient.CreateUserAsync(new CreateUserDto
{
    FirstName = "Foo",
    LastName = "Bar",
    DisplayName = "Barry",
    Ssn = "123456",
    Email = "barry@email.com",
    Password = "Abc123!?"
});

Console.WriteLine("Created user");

static IServiceProvider BuildServiceProvider()
{
    ServiceCollection services = new();

    services.AddHttpClient(nameof(IUsersClient), (sp, http) =>
    {
        http.BaseAddress = new Uri($"https://identity.local/");
        http.DefaultRequestHeaders.Add("X-API-KEY", ApiKey);
    })
    .AddTypedClient<IUsersClient>((http, sp) => new UsersClient(http));

    /*
    services.AddHttpClient(nameof(Skynet.TimeReport.Client.IProjectsClient), (sp, http) =>
    {
        http.BaseAddress = new Uri($"https://localhost:5050/");
        http.DefaultRequestHeaders.Add("X-API-KEY", ApiKey);
    })
    .AddTypedClient<Skynet.TimeReport.Client.IProjectsClient>((http, sp) => new Skynet.TimeReport.Client.ProjectsClient(http));
    */

    return services.BuildServiceProvider();
}