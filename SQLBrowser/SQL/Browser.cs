using DevFromDownUnder.SQLBrowser.Extensions;
using DevFromDownUnder.SQLBrowser.SQL;
using DevFromDownUnder.SQLBrowser.SQL.Local;
using DevFromDownUnder.SQLBrowser.SQL.Network;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;

namespace DevFromDownUnder.SQLBrowser
{
    public class Browser
    {
        public event EventHandler<Server> OnServerDiscovered;

        public event EventHandler<LocalServer> OnLocalServerDiscovered;

        public event EventHandler<NetworkServer> OnNetworkServerDiscovered;

        private readonly ILogger _logger;
        private NetworkDiscovery networkDiscovery;
        private LocalDiscovery localDiscovery;

        public Browser(ILogger logger = default)
        {
            _logger = logger;
        }

        [SupportedOSPlatform("windows")]
        public async Task<Server[]> DiscoverServers()
        {
            using var _ = new CancellationTokenSource();
            return await DiscoverServers(_.Token).ConfigureAwait(false);
        }

        [SupportedOSPlatform("windows")]
        public async Task<Server[]> DiscoverServers(CancellationToken cancellationToken, NetworkConfiguration configuration = null)
        {
            _logger?.LogDebug(LoggingExtensions.CurrentFunction());

            _logger?.LogInformation("Discovery started");

            var networkTask = DiscoverNetworkServers(cancellationToken, configuration);
            var localTask = DiscoverLocalServers(cancellationToken);

            var results = await Task.WhenAll<Server[]>(networkTask, localTask);

            _logger?.LogInformation("Discovery ended");

            return results.Where(s => s != null).SelectMany(s => s).Distinct().ToArray();
        }

        public async Task<Server[]> DiscoverNetworkServers(CancellationToken cancellationToken, NetworkConfiguration configuration = null)
        {
            _logger?.LogDebug(LoggingExtensions.CurrentFunction());

            networkDiscovery = new(_logger, configuration);
            networkDiscovery.OnNetworkServerDiscovered += OnNetworkServerDiscovered;
            networkDiscovery.OnServerDiscovered += OnServerDiscovered;

            return await networkDiscovery.DiscoverAsync(cancellationToken).ConfigureAwait(false); ;
        }

        [SupportedOSPlatform("windows")]
        public async Task<Server[]> DiscoverLocalServers(CancellationToken cancellationToken)
        {
            _logger?.LogDebug(LoggingExtensions.CurrentFunction());

            localDiscovery = new(_logger);
            localDiscovery.OnLocalServerDiscovered += OnLocalServerDiscovered;
            localDiscovery.OnServerDiscovered += OnServerDiscovered;

            return await localDiscovery.DiscoverAsync(cancellationToken).ConfigureAwait(false); ;
        }

        public void Stop()
        {
            _logger?.LogDebug(LoggingExtensions.CurrentFunction());

            _logger?.LogInformation("Discovery cancelled");

            networkDiscovery?.Stop();
            localDiscovery?.Stop();
        }
    }
}