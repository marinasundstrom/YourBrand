using Microsoft.Extensions.Primitives;

using Steeltoe.Discovery;
using Steeltoe.Discovery.Consul.Discovery;

using Yarp.ReverseProxy.Configuration;

namespace BlazorApp.Extensions;

public class InMemoryConfigProvider : IProxyConfigProvider, IHostedService, IDisposable
{
    private Timer _timer;
    private volatile InMemoryConfig _config;
    private readonly ConsulDiscoveryClient _discoveryClient;
    private readonly RouteConfig[] _routes;

    public InMemoryConfigProvider(IDiscoveryClient discoveryClient)
    {
        _discoveryClient = discoveryClient as Steeltoe.Discovery.Consul.Discovery.ConsulDiscoveryClient;

        _routes = new[]
                   {
                        new RouteConfig()
                        {
                            RouteId = "storefront_route",
                            ClusterId = "yourbrand-storefront-svc",
                            Match = new RouteMatch
                            {
                                Path = "storefront/{**catchall}"
                            },
                            Transforms =  new List<Dictionary<string, string>>
                            {
                                new Dictionary<string, string>
                                {
                                    { "PathPattern", "{**catchall}"}
                                }
                            }
                        }
                    };

        PopulateConfig();
    }

    private void Update(object state)
    {
        PopulateConfig();
    }

    private void PopulateConfig()
    {
        var apps = _discoveryClient.GetServices();
        List<ClusterConfig> clusters = new();

        foreach (var app in apps)
        {
            var cluster = new ClusterConfig
            {
                ClusterId = app,
                Destinations = _discoveryClient.GetInstances(app)
                .Select(x =>
                    (  ServiceId: Guid.NewGuid().ToString(),
                        new DestinationConfig()
                        {
                            Address = $"https://{x.Host}:{x.Port}"
                        }))
                .ToDictionary(y => y.ServiceId, y => y.Item2)
            };

            clusters.Add(cluster);
        }

        var oldConfig = _config;
        _config = new InMemoryConfig(_routes, clusters);
        oldConfig?.SignalChange();
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }


    public IProxyConfig GetConfig() => _config;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(Update, null, TimeSpan.Zero, TimeSpan.FromSeconds(30));
        return Task.CompletedTask;
    }


    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }


    private class InMemoryConfig : IProxyConfig
    {
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public InMemoryConfig(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters)
        {
            Routes = routes;
            Clusters = clusters;
            ChangeToken = new CancellationChangeToken(_cts.Token);
        }


        public IReadOnlyList<RouteConfig> Routes { get; }

        public IReadOnlyList<ClusterConfig> Clusters { get; }

        public IChangeToken ChangeToken { get; }

        internal void SignalChange()
        {
            _cts.Cancel();
        }
    }
}