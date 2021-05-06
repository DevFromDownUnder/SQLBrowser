using DevFromDownUnder.SQLBrowser.Extensions;
using System;

namespace DevFromDownUnder.SQLBrowser.SQL
{
    public class Server
    {
        public enum ServerType
        {
            Unknown,
            Local,
            Network
        }

        private const string DEFAULT_INSTANCE = "MSSQLSERVER";

        public string ServerName
        {
            get
            {
                if (InstanceName.HasNoValue() || string.Equals(InstanceName, DEFAULT_INSTANCE, StringComparison.OrdinalIgnoreCase))
                {
                    return HostName ?? string.Empty;
                }
                else
                {
                    return $"{HostName ?? string.Empty}\\{InstanceName}";
                }
            }
        }

#pragma warning disable IDE1006 // Naming Styles
        public ServerType Type { get; set; }
        public string HostName { get; set; }
        public string InstanceName { get; set; }
        public bool IsClustered { get; set; }
        public string VersionString { get; set; }
        public ServerVersions.Version Version { get; set; }
        public string NamedPipe { get; set; }
        public bool NamedPipeEnabled { get; set; }
        public ushort? TCPPort { get; set; }
        public string via { get; set; }
        public string rpc { get; set; }
        public string spx { get; set; }
        public string adsp { get; set; }
        public string bv { get; set; }
#pragma warning restore IDE1006 // Naming Styles

        #region "Equality logic based on ServerName"

        public override string ToString()
        {
            return ServerName;
        }

        public override bool Equals(object obj)
        {
            if (obj is Server other)
            {
                return String.Equals(this.ServerName, other.ServerName, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return ServerName.GetHashCode();
        }

        #endregion "Equality logic based on ServerName"
    }
}