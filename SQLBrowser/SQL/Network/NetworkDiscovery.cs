using DevFromDownUnder.SQLBrowser.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace DevFromDownUnder.SQLBrowser.SQL.Network
{
    public class NetworkDiscovery
    {
        public event EventHandler<Server> OnServerDiscovered;

        public event EventHandler<NetworkServer> OnNetworkServerDiscovered;

        private readonly ILogger _logger;

        private CancellationTokenSource discoveryCancellationTokenSource;
        private readonly UdpClient client;

        public NetworkConfiguration Configuration { get; set; }

        public NetworkDiscovery(ILogger logger = default, NetworkConfiguration configuration = default)
        {
            _logger = logger;

            _logger.LogDebug(LoggingExtensions.CurrentFunction());

            if (configuration == null)
            {
                configuration = new();
            }

            Configuration = configuration;

            client = new()
            {
                EnableBroadcast = true
            };
        }

        public async Task<Server[]> Discover()
        {
            return await DiscoverAsync(new CancellationTokenSource().Token).ConfigureAwait(false);
        }

        public async Task<Server[]> DiscoverAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug(LoggingExtensions.CurrentFunction());

            Server[] servers = null;

            discoveryCancellationTokenSource?.Cancel();
            discoveryCancellationTokenSource = new();

            _logger.LogInformation("Network discovery started");

            discoveryCancellationTokenSource.CancelAfter(Configuration.Timeout);

            using (var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(discoveryCancellationTokenSource.Token, cancellationToken))
            {
                var linkedCancellationToken = linkedCancellationTokenSource.Token;

                _ = BroadcastAsync(linkedCancellationToken).ConfigureAwait(false);

                servers = await ListenAsync(linkedCancellationToken).ConfigureAwait(false);
            }

            _logger.LogInformation("Network discovery ended");

            discoveryCancellationTokenSource.Cancel();

            return servers;
        }

        public void Stop()
        {
            _logger.LogDebug(LoggingExtensions.CurrentFunction());

            _logger.LogInformation("Network discovery cancelled");

            discoveryCancellationTokenSource?.Cancel();
        }

        private async Task BroadcastAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug(LoggingExtensions.CurrentFunction());

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await client.SendAsync(Architecture.CLNT_BCAST_EX, Architecture.CLNT_BCAST_EX.Length, new IPEndPoint(IPAddress.Broadcast, Configuration.Port)).WithCancellation(cancellationToken).ConfigureAwait(false);
                }
                catch (TaskCanceledException)
                {
                    _logger.LogDebug($"[{LoggingExtensions.CurrentFunction()}] Cancelled");
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Error occured while sending discovery requests");

                    if (Configuration.SendExceptionAction is Architecture.ExceptionActions.LogAndThrow)
                    {
                        throw;
                    }
                }

                if (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        await Task.Delay(Configuration.ResendDelay, cancellationToken).ConfigureAwait(false);
                    }
                    catch (TaskCanceledException)
                    {
                        _logger.LogDebug($"[{LoggingExtensions.CurrentFunction()}] Cancelled");
                    }
                    catch { }
                }
            }
        }

        private async Task<Server[]> ListenAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug(LoggingExtensions.CurrentFunction());

            var servers = new ConcurrentBag<Server>();

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var result = await client.ReceiveAsync().WithCancellation(cancellationToken).ConfigureAwait(false);

                    _ = ProcessResponse(servers, result.Buffer, result.RemoteEndPoint, cancellationToken).ConfigureAwait(false);
                }
                catch (TaskCanceledException)
                {
                    _logger.LogDebug($"[{LoggingExtensions.CurrentFunction()}] Cancelled");
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Error occured while receiving responses");

                    if (Configuration.ReceiveExceptionAction is Architecture.ExceptionActions.LogAndThrow)
                    {
                        throw;
                    }
                }
            }

            return servers.Distinct().ToArray();
        }

        private Task ProcessResponse(ConcurrentBag<Server> servers, byte[] buffer, IPEndPoint responder, CancellationToken cancellationToken)
        {
            _logger.LogDebug(LoggingExtensions.CurrentFunction());

            return Task.Run(() =>
            {
                try
                {
                    var response = SQL.Network.NetworkServerResponse.Parse(buffer, responder);
                    if (response == null)
                    {
                        return;
                    }

                    var networkServers = SQL.Network.NetworkServers.Parse(response);
                    if (networkServers.Count == 0)
                    {
                        return;
                    }

                    foreach (var networkServer in networkServers)
                    {
                        var server = networkServer.ToServer();

                        servers.Add(server);

                        OnNetworkServerDiscovered?.Invoke(this, networkServer);
                        OnServerDiscovered?.Invoke(this, server);
                    }
                }
                catch (TaskCanceledException)
                {
                    _logger.LogDebug($"[{LoggingExtensions.CurrentFunction()}] Cancelled");
                }
            }, cancellationToken);
        }
    }
}