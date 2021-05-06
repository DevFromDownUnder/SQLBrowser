using DevFromDownUnder.SQLBrowser.Extensions;
using Microsoft.Win32;
using System.Runtime.Versioning;

namespace DevFromDownUnder.SQLBrowser.SQL.Local
{
    [SupportedOSPlatform("windows")]
    public class LocalServer
    {
        public const string REG_SERVER_ROOT_PATH = "Software\\Microsoft\\Microsoft SQL Server";
        public const string REG_SERVER_KEY_LOOKUP_PATH = "Instance Names\\SQL";
        public const string REG_SERVER_VERSION_PATH = "MSSQLServer\\CurrentVersion";
        public const string REG_SERVER_VERSION_KEY = "CurrentVersion";
        public const string REG_SERVER_NAMED_PIPE_PATH = "MSSQLServer\\SuperSocketNetLib\\Np";
        public const string REG_SERVER_NAMED_PIPE_ENABLED_KEY = "Enabled";
        public const string REG_SERVER_NAMED_PIPE_NAME_KEY = "PipeName";
        public const string REG_SERVER_TCP_IPALL_PATH = "MSSQLServer\\SuperSocketNetLib\\Tcp\\IPAll";
        public const string REG_SERVER_TCP_DYNAMIC_PORT_KEY = "TcpDynamicPorts";
        public const string REG_SERVER_CLUSTER_PATH = "Cluster";
        public const string REG_SERVER_CLUSTER_NAME_KEY = "ClusterName";

        public const string LOCAL_HOST_NAME = ".";

        public string InstanceName { get; set; }
        public string InstanceKey { get; set; }
        public string Version { get; set; }
        public string NamedPipe { get; set; }
        public bool NamedPipeEnabled { get; set; }
        public ushort? TCPPort { get; set; }
        public bool IsClustered { get; set; }

        public void Populate(RegistryKey hive, string instance)
        {
            InstanceName = instance;
            InstanceKey = LookupInstanceKey(hive, instance);

            //The rest is dependent on the key lookup
            if (InstanceKey != null)
            {
                Version = LookupVersion(hive, InstanceKey);
                NamedPipeEnabled = LookupNamedPipeEnabled(hive, InstanceKey);
                NamedPipe = LookupNamedPipeName(hive, InstanceKey);
                TCPPort = LookupTCPPort(hive, InstanceKey);
                IsClustered = LookupIsClustered(hive, InstanceKey);
            }
        }

        public static string LookupInstanceKey(RegistryKey hive, string instance)
        {
            using var key = hive.OpenSubKey($"{REG_SERVER_ROOT_PATH}\\{REG_SERVER_KEY_LOOKUP_PATH}");

            return key?.GetValue($"{instance}") as string;
        }

        public static string LookupVersion(RegistryKey hive, string instanceKey)
        {
            using var key = hive.OpenSubKey($"{REG_SERVER_ROOT_PATH}\\{instanceKey}\\{REG_SERVER_VERSION_PATH}");

            return key?.GetValue($"{REG_SERVER_VERSION_KEY}") as string;
        }

        public static bool LookupNamedPipeEnabled(RegistryKey hive, string instanceKey)
        {
            using var key = hive.OpenSubKey($"{REG_SERVER_ROOT_PATH}\\{instanceKey}\\{REG_SERVER_NAMED_PIPE_PATH}");

            return key?.GetValue($"{REG_SERVER_NAMED_PIPE_ENABLED_KEY}") as bool? ?? false;
        }

        public static string LookupNamedPipeName(RegistryKey hive, string instanceKey)
        {
            using var key = hive.OpenSubKey($"{REG_SERVER_ROOT_PATH}\\{instanceKey}\\{REG_SERVER_NAMED_PIPE_PATH}");

            return key?.GetValue($"{REG_SERVER_NAMED_PIPE_NAME_KEY}") as string;
        }

        public static ushort? LookupTCPPort(RegistryKey hive, string instanceKey)
        {
            using var key = hive.OpenSubKey($"{REG_SERVER_ROOT_PATH}\\{instanceKey}\\{REG_SERVER_TCP_IPALL_PATH}");

            return key?.GetValue($"{REG_SERVER_TCP_DYNAMIC_PORT_KEY}") as ushort?;
        }

        public static bool LookupIsClustered(RegistryKey hive, string instanceKey)
        {
            //Not certain on this one, don't have an easy way to set it up to test
            //I beleive you should have a value in Cluster\ClusterName to be clustered
            //Going off an except from https://github.com/dsccommunity/SqlServerDsc/issues/1232

            //Another possibility would be values in {REG_SERVER_ROOT_PATH}\\{instanceKey}\\ConfigurationState
            //But the below looked nicer to work with

            using var key = hive.OpenSubKey($"{REG_SERVER_ROOT_PATH}\\{instanceKey}\\{REG_SERVER_CLUSTER_PATH}");

            return (key?.GetValue($"{REG_SERVER_CLUSTER_NAME_KEY}") as string).HasValue();
        }

        public Server ToServer()
        {
            return ToServer(this);
        }

        public static Server ToServer(LocalServer server)
        {
            return new Server()
            {
                adsp = default,
                bv = default,
                HostName = LOCAL_HOST_NAME,
                InstanceName = server.InstanceName,
                IsClustered = server.IsClustered,
                NamedPipe = server.NamedPipe,
                NamedPipeEnabled = server.NamedPipeEnabled,
                rpc = default,
                spx = default,
                TCPPort = server.TCPPort,
                Type = Server.ServerType.Local,
                Version = ServerVersions.GetKnownVersion(server.Version),
                VersionString = server.Version,
                via = default
            };
        }
    }
}