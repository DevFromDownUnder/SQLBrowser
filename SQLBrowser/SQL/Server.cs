using System;

namespace DevFromDownUnder.SQLBrowser.SQL
{
    public class Server
    {
        public const string DELIMITER = ";";

        private const string REMOVE_DEFAULT_INSTANCE = "MSSQLSERVER";

        public ServerResponse Response { get; set; }

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

        public override string ToString()
        {
            return string.IsNullOrWhiteSpace(InstanceName) ? ServerName ?? "" : ServerName + "\\" + InstanceName;
        }

        public override bool Equals(object obj)
        {
            if (obj is Server other)
            {
                return this.ToString().Equals(other.ToString());
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public static Server Parse(ServerResponse response, string data)
        {
            var result = new Server() { Response = response };

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

                                if (result.InstanceName == REMOVE_DEFAULT_INSTANCE)
                                {
                                    result.InstanceName = string.Empty;
                                }

                                break;

                            case nameof(IsClustered):
                                result.IsClustered = (tokens[i + 1]?.ToLower() ?? "no") == "yes";
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
    }
}