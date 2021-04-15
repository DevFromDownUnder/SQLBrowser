using System;
using SQLBrowser.SQL;
using System.Threading;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Net;
using System.Text;
using System.Collections.Concurrent;

namespace SQLBrowser
{
    public class Browser
    {
        public event EventHandler<Server> OnSQLServerDiscovered;

        private ILogger<Browser> _logger;

        private CancellationTokenSource discoveryCancellationTokenSource;
        private CancellationToken discoveryCancellationToken;
        private int discoveryPort;
        private UdpClient client;

        public int Timeout { get; set; } = 2000;
        public int ResendDelay { get; set; } = 500;

        public Discovery.ExceptionActions SendExceptionAction { get; set; } = Discovery.ExceptionActions.None;
        public Discovery.ExceptionActions ReceiveExceptionAction { get; set; } = Discovery.ExceptionActions.None;

        public Browser() => Initialize(null, Discovery.DEFAULT_UDP_PORT);

        public Browser(ILogger<Browser> logger) => Initialize(logger, Discovery.DEFAULT_UDP_PORT);

        public Browser(int port) => Initialize(null, port);

        public Browser(ILogger<Browser> logger, int port) => Initialize(logger, port);

        private void Initialize(ILogger<Browser> logger, int port)
        {
            _logger = logger;
            discoveryPort = port;

            _logger?.LogInformation($"Browser created. Port: {discoveryPort}");

            client = new()
            {
                EnableBroadcast = true
            };
        }

        public async Task<Server[]> Discover()
        {
            using (var _ = new CancellationTokenSource())
            {
                return await Discover(_.Token);
            }
        }

        public async Task<Server[]> Discover(CancellationToken cancellationToken)
        {
            Server[] servers = default;

            discoveryCancellationTokenSource?.Cancel();
            discoveryCancellationTokenSource = new(Timeout);

            using (var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(discoveryCancellationTokenSource.Token, cancellationToken))
            {
                var linkedCancellationToken = linkedCancellationTokenSource.Token;

                _ = Broadcast(linkedCancellationToken);

                servers = await Listen(linkedCancellationToken);
            }

            discoveryCancellationTokenSource.Cancel();

            return servers;
        }

        private async Task Broadcast(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await client.SendAsync(Discovery.DISCOVERY_PAYLOAD, Discovery.DISCOVERY_PAYLOAD.Length, new IPEndPoint(IPAddress.Broadcast, discoveryPort)).ConfigureAwait(false);
                }
                catch (TaskCanceledException) { }
                catch (Exception ex)
                {
                    if (SendExceptionAction is Discovery.ExceptionActions.Log or Discovery.ExceptionActions.LogAndThrow)
                    {
                        _logger?.LogError(ex, "Error occured while sending discovery requests");
                    }

                    if (SendExceptionAction is Discovery.ExceptionActions.Throw or Discovery.ExceptionActions.LogAndThrow)
                    {
                        throw;
                    }
                }

                if (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        await Task.Delay(ResendDelay, cancellationToken).ConfigureAwait(false);
                    }
                    catch { }
                }
            }
        }

        private async Task<Server[]> Listen(CancellationToken cancellationToken)
        {
            var servers = new ConcurrentBag<Server>();

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var result = await client.ReceiveAsync().ConfigureAwait(false);
                    var response = Encoding.ASCII.GetString(result.Buffer);

                    _ = Task.Run(() =>
                    {
                        try
                        {
                            var server = Server.ParseServer(result.RemoteEndPoint.ToString(), response);
                            if (server != null)
                            {
                                OnSQLServerDiscovered?.Invoke(this, server);
                            }
                        }
                        catch (TaskCanceledException) { }
                    }, cancellationToken);
                }
                catch (TaskCanceledException) { }
                catch (Exception ex)
                {
                    if (ReceiveExceptionAction is Discovery.ExceptionActions.Log or Discovery.ExceptionActions.LogAndThrow)
                    {
                        _logger?.LogError(ex, "Error occured while receiving responses");
                    }

                    if (ReceiveExceptionAction is Discovery.ExceptionActions.Throw or Discovery.ExceptionActions.LogAndThrow)
                    {
                        throw;
                    }
                }
            }

            return servers.ToArray();
        }

        public void Stop()
        {
            discoveryCancellationTokenSource?.Cancel();
        }
    }
}