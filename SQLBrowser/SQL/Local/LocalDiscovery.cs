using DevFromDownUnder.SQLBrowser.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;

namespace DevFromDownUnder.SQLBrowser.SQL.Local
{
    public class LocalDiscovery
    {
        public event EventHandler<Server> OnServerDiscovered;

        public event EventHandler<LocalServer> OnLocalServerDiscovered;

        private readonly ILogger _logger;

        private CancellationTokenSource discoveryCancellationTokenSource;

        public LocalDiscovery(ILogger logger)
        {
            _logger = logger;

            _logger.LogDebug(LoggingExtensions.CurrentFunction());
        }

        [SupportedOSPlatform("windows")]
        public async Task<Server[]> Discover()
        {
            return await DiscoverAsync(new CancellationTokenSource().Token).ConfigureAwait(false);
        }

        [SupportedOSPlatform("windows")]
        public async Task<Server[]> DiscoverAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug(LoggingExtensions.CurrentFunction());

            Server[] servers = default;

            discoveryCancellationTokenSource?.Cancel();
            discoveryCancellationTokenSource = new();

            _logger.LogInformation("Local discovery started");

            using (var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(discoveryCancellationTokenSource.Token, cancellationToken))
            {
                var linkedCancellationToken = linkedCancellationTokenSource.Token;

                servers = await GetServersAsync(linkedCancellationToken).ConfigureAwait(false);
            }

            _logger.LogInformation("Local discovery ended");

            discoveryCancellationTokenSource.Cancel();

            return servers;
        }

        public void Stop()
        {
            _logger.LogDebug(LoggingExtensions.CurrentFunction());

            _logger.LogInformation("Local discovery cancelled");

            discoveryCancellationTokenSource?.Cancel();
        }

        [SupportedOSPlatform("windows")]
        private async Task<Server[]> GetServersAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug(LoggingExtensions.CurrentFunction());

            return await Task.Run(async () =>
            {
                var servers = new ConcurrentBag<Server>();

                try
                {
                    var populationTasks = new List<Task>();

                    var registryInstances = LocalServers.LookupRegistryInstances();

                    foreach (var (hive, instances) in registryInstances)
                    {
                        foreach (var instance in instances)
                        {
                            populationTasks.Add(PopulateLocalServerAsync(servers, hive, instance, cancellationToken));
                        }
                    }

                    await Task.WhenAll(populationTasks).ConfigureAwait(false);
                }
                catch (TaskCanceledException)
                {
                    _logger.LogDebug($"[{LoggingExtensions.CurrentFunction()}] Cancelled");
                }

                return servers.Distinct().ToArray();
            }, cancellationToken).ConfigureAwait(false);
        }

        [SupportedOSPlatform("windows")]
        private Task PopulateLocalServerAsync(ConcurrentBag<Server> servers, RegistryKey hive, string instance, CancellationToken cancellationToken)
        {
            _logger.LogDebug(LoggingExtensions.CurrentFunction());

            return Task.Run(() =>
            {
                try
                {
                    var localServer = new LocalServer();

                    localServer.Populate(hive, instance);

                    var server = localServer.ToServer();

                    servers.Add(server);

                    OnLocalServerDiscovered?.Invoke(this, localServer);
                    OnServerDiscovered?.Invoke(this, server);
                }
                catch (TaskCanceledException)
                {
                    _logger.LogDebug($"[{LoggingExtensions.CurrentFunction()}] Cancelled");
                }
            }, cancellationToken);
        }
    }
}