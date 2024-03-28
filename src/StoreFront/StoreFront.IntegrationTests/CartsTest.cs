using MassTransit;
using MassTransit.Testing;

using Meziantou.Extensions.Logging.Xunit;

using Microsoft.AspNetCore.Mvc.Testing;

using YourBrand.StoreFront.Contracts;

using Xunit.Abstractions;

namespace YourBrand.StoreFront.IntegrationTests;

[Collection("Database collection")]
public class StoreFrontTest : IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Func<Task> _resetDatabase;

    public HttpClient HttpClient { get; private set; }
    public ITestHarness Harness { get; private set; }

    public StoreFrontTest(StoreFrontApiFactory factory, ITestOutputHelper testOutputHelper)
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
    public async Task AddNewCartItem()
    {
        // Arrange

        var requestClient = Harness.GetRequestClient<AddCartItem>();

        // Act

        var response = await requestClient.GetResponse<AddCartItemResponse>(
            new AddCartItem
            {
                CartId = "test",
                Name = "Test",
                ProductId = 100,
                ProductHandle = "foo",
                Description = "",
                Price = 20,
                Quantity = 1
            });

        // Assert

        Assert.True(await Harness.Consumed.Any<AddCartItem>());

        Assert.True(await Harness.Sent.Any<AddCartItemResponse>());
    }

    public async Task DisposeAsync() => await _resetDatabase();
}