using DevFromDownUnder.SQLBrowser.Extensions;
using System;

namespace DevFromDownUnder.SQLBrowser.SQL.Network
{
    public class NetworkServer
    {
        public const string DELIMITER = ";";

        public NetworkServerResponse Response { get; set; }

        //See MC-SQLR spec
#pragma warning disable IDE1006 // Naming Styles
        public string ServerName { get; set; }
        public string InstanceName { get; set; }
        public bool IsClustered { get; set; }
        public string Version { get; set; }
        public string np { get; set; }
        public string tcp { get; set; }
        public string via { get; set; }
        public string rpc { get; set; }
        public string spx { get; set; }
        public string adsp { get; set; }
        public string bv { get; set; }
#pragma warning restore IDE1006 // Naming Styles

        public static NetworkServer Parse(NetworkServerResponse response, string data)
        {
            var result = new NetworkServer() { Response = response };

            if (data.Contains(DELIMITER))
            {
                var tokens = data.Split(DELIMITER, StringSplitOptions.RemoveEmptyEntries);

                for (var i = 0; i < tokens.Length; i += 2)
                {
                    //Must have a matching pair
                    if (i + 1 < tokens.Length)
                    {
                        switch (tokens[i])
                        {
                            case nameof(ServerName):
                                result.ServerName = tokens[i + 1];
                                break;

                            case nameof(InstanceName):
                                result.InstanceName = tokens[i + 1];
                                break;

                            case nameof(IsClustered):
                                result.IsClustered = String.Equals(tokens[i + 1], "yes", StringComparison.OrdinalIgnoreCase);
                                break;

                            case nameof(Version):
                                result.Version = tokens[i + 1];
                                break;

                            case nameof(np):
                                result.np = tokens[i + 1];
                                break;

                            case nameof(tcp):
                                result.tcp = tokens[i + 1];
                                break;

                            case nameof(via):
                                result.via = tokens[i + 1];
                                break;

                            case nameof(rpc):
                                result.rpc = tokens[i + 1];
                                break;

                            case nameof(spx):
                                result.spx = tokens[i + 1];
                                break;

                            case nameof(adsp):
                                result.adsp = tokens[i + 1];
                                break;

                            case nameof(bv):
                                result.bv = tokens[i + 1];
                                break;
                        }
                    }
                }
            }

            return result;
        }

        public Server ToServer()
        {
            return ToServer(this);
        }

        public static Server ToServer(NetworkServer server)
        {
            return new Server()
            {
                adsp = server.adsp,
                bv = server.bv,
                HostName = server.ServerName,
                InstanceName = server.InstanceName,
                IsClustered = server.IsClustered,
                NamedPipe = server.np,
                NamedPipeEnabled = server.np.HasValue(),
                rpc = server.rpc,
                spx = server.spx,
                TCPPort = ushort.TryParse(server.tcp, out ushort port) ? port : null,
                Type = Server.ServerType.Network,
                Version = ServerVersions.GetKnownVersion(server.Version),
                VersionString = server.Version,
                via = server.via
            };
        }
    }
}