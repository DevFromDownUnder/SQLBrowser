using DevFromDownUnder.SQLBrowser.Extensions;
using DevFromDownUnder.SQLBrowser.SQL;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace DevFromDownUnder.SQLBrowser
{
    public class Browser
    {
        /// <summary>
        /// Can return duplicates if ResendDelay &lt; Timeout as there is resend logic
        /// </summary>
        public event EventHandler<Server> OnSQLServerDiscovered;

        private ILogger _logger;

        private CancellationTokenSource discoveryCancellationTokenSource;
        private int discoveryPort;
        private UdpClient client;

        public int Timeout { get; set; } = 2000;
        public int ResendDelay { get; set; } = 500;

        public Discovery.ExceptionActions SendExceptionAction { get; set; } = Discovery.ExceptionActions.None;
        public Discovery.ExceptionActions ReceiveExceptionAction { get; set; } = Discovery.ExceptionActions.None;

        public Browser() => Initialize(null, Discovery.DEFAULT_UDP_PORT);

        public Browser(ILogger logger) => Initialize(logger, Discovery.DEFAULT_UDP_PORT);

        public Browser(int port) => Initialize(null, port);

        public Browser(ILogger logger, int port) => Initialize(logger, port);

        private void Initialize(ILogger logger, int port)
        {
            _logger = logger;
            discoveryPort = port;

            LogAction("Browser", $"port: {port}");

            client = new()
            {
                EnableBroadcast = true
            };
        }

        public async Task<Server[]> Discover()
        {
            using var _ = new CancellationTokenSource();
            return await Discover(_.Token).ConfigureAwait(false);
        }

        public async Task<Server[]> Discover(CancellationToken cancellationToken)
        {
            Server[] servers = default;

            discoveryCancellationTokenSource?.Cancel();
            discoveryCancellationTokenSource = new();

            LogAction("Started");

            discoveryCancellationTokenSource.CancelAfter(Timeout);

            using (var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(discoveryCancellationTokenSource.Token, cancellationToken))
            {
                var linkedCancellationToken = linkedCancellationTokenSource.Token;

                _ = Broadcast(linkedCancellationToken).ConfigureAwait(false);

                servers = await Listen(linkedCancellationToken).ConfigureAwait(false);
            }

            LogAction("Ended");

            discoveryCancellationTokenSource.Cancel();

            return servers;
        }

        private async Task Broadcast(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await client.SendAsync(Discovery.CLNT_BCAST_EX, Discovery.CLNT_BCAST_EX.Length, new IPEndPoint(IPAddress.Broadcast, discoveryPort)).WithCancellation(cancellationToken).ConfigureAwait(false);
                }
                catch (TaskCanceledException) { LogCancellation(); }
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
                    catch (TaskCanceledException) { LogCancellation(); }
                    catch { }
                }
            }
        }

        private async Task<Server[]> Listen(CancellationToken cancellationToken)
        {
            var serverList = new ConcurrentBag<Server>();

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var result = await client.ReceiveAsync().WithCancellation(cancellationToken).ConfigureAwait(false);

                    _ = ProcessResponse(serverList, result.Buffer, result.RemoteEndPoint, cancellationToken).ConfigureAwait(false);
                }
                catch (TaskCanceledException) { LogCancellation(); }
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

            return serverList.Distinct().ToArray();
        }

        private Task ProcessResponse(ConcurrentBag<Server> serverList, byte[] buffer, IPEndPoint responder, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                try
                {
                    var response = ServerResponse.Parse(buffer, responder);
                    if (response == null)
                    {
                        return;
                    }

                    var servers = Servers.Parse(response);
                    if (servers.Count == 0)
                    {
                        return;
                    }

                    foreach (var server in servers)
                    {
                        serverList.Add(server);

                        OnSQLServerDiscovered?.Invoke(this, server);
                    }
                }
                catch (TaskCanceledException) { LogCancellation(); }
            }, cancellationToken);
        }

        private void LogCancellation([System.Runtime.CompilerServices.CallerMemberName] string name = "")
        {
            LogAction("Cancelled", "", name);
        }

        private void LogAction(string action, string addendum = "", [System.Runtime.CompilerServices.CallerMemberName] string name = "")
        {
            _logger?.LogInformation($"({name}) => {action}{(addendum != "" ? " - " + addendum : "")}");
        }

        public void Stop()
        {
            LogAction("User Cancelled");

            discoveryCancellationTokenSource?.Cancel();
        }
    }
}