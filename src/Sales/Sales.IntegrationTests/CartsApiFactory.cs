
using DotNet.Testcontainers.Builders;

using MassTransit;
using MassTransit.Testing;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

using Respawn;

using YourBrand.Sales.Persistence;

using Testcontainers.MsSql;

namespace YourBrand.Carts.IntegrationTests;

public class CartsApiFactory
    : WebApplicationFactory<Program>, IAsyncLifetime
{
    private const string CartsDbName = "yourbrand-carts-db";
    private const string DbServerName = "yourbrand-test-sqlserver";
    static readonly SqlEdgeContainer _dbContainer = new SqlEdgeBuilder()
        .WithImage("mcr.microsoft.com/azure-sql-edge:1.0.7")
        .WithHostname(DbServerName)
        .WithName(DbServerName)
        .WithPortBinding(51736)
        .Build();
    private SqlConnection _dbConnection;
    private Respawner _respawner;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(async services =>
        {
            var descriptor = services.Single(
        d => d.ServiceType ==
            typeof(DbContextOptions<SalesContext>));

            services.Remove(descriptor);

            services.AddDbContext<SalesContext>((sp, options) =>
            {
                var connectionString = _dbContainer.GetConnectionString().Replace("master", CartsDbName);
                options.UseSqlServer(connectionString);

#if DEBUG
            options.EnableSensitiveDataLogging();
#endif
            });

            services.AddMassTransitTestHarness(x =>
            {
                x.AddDelayedMessageScheduler();

                x.AddConsumers(typeof(Sales.API.Features.OrderManagement.Orders.Mappings).Assembly);

                x.UsingInMemory((context, cfg) =>
                {
                    cfg.UseDelayedMessageScheduler();

                    cfg.UseTenancyFilters(context);
                    cfg.UseIdentityFilters(context);

                    cfg.ConfigureEndpoints(context);
                });
            });

            var sp = services.BuildServiceProvider();

            using (var scope = sp.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<SalesContext>();
                var configuration = scopedServices.GetRequiredService<IConfiguration>();

                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                try
                {
                    await Seed.SeedData(db, configuration);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        });

        builder.UseEnvironment("Development");

        builder.ConfigureTestServices(services =>
        {

        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        _dbConnection = new SqlConnection(_dbContainer.GetConnectionString().Replace("Database=master;", string.Empty));
        await InitializeRespawner();
    }

    public async Task InitializeRespawner()
    {
        await _dbConnection.OpenAsync();
        _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.SqlServer
        });
    }

    public async Task ResetDatabaseAsync()
    {
        await _respawner.ResetAsync(_dbConnection);
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync()
        .ConfigureAwait(false);
    }
}