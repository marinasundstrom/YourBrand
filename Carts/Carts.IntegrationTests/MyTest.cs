using YourBrand.Carts.Contracts;

using MassTransit;
using MassTransit.Testing;

using Meziantou.Extensions.Logging.Xunit;

using Microsoft.AspNetCore.Mvc.Testing;

using Xunit.Abstractions;

namespace YourBrand.Carts.IntegrationTests;

[Collection("Database collection")]
public class MyTest : IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Func<Task> _resetDatabase;

    public HttpClient HttpClient { get; private set; }
    public ITestHarness Harness { get; private set; }

    public MyTest(CartsApiFactory factory, ITestOutputHelper testOutputHelper)
    {
        _factory = factory.WithTestLogging(testOutputHelper);

        _resetDatabase = factory.ResetDatabaseAsync;
    }

    public async Task InitializeAsync()
    {
        HttpClient = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        Harness = _factory.Services.GetTestHarness();
        await Harness.Start();
    }

    [Fact]
    public async Task GetCarts()
    {
        // Arrange


        // Act
        var result = await HttpClient.GetStringAsync("/api/carts");

        // Assert
    }

    public async Task DisposeAsync() => await _resetDatabase();
}