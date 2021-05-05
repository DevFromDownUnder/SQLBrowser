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

        public const string LOCAL_HOST_NAME = ".";

        public string InstanceName { get; set; }
        public string InstanceKey { get; set; }
        public string Version { get; set; }

        public void Populate(RegistryKey hive, string instance)
        {
            InstanceName = instance;
            InstanceKey = LookupInstanceKey(hive, instance);

            //The rest is dependent on the key lookup
            if (InstanceKey != null)
            {
                Version = LookupVersion(hive, InstanceKey);
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

        public Server ToServer()
        {
            return ToServer(this);
        }

        public static Server ToServer(LocalServer server)
        {
            return new Server()
            {
                adsp = string.Empty,
                bv = string.Empty,
                HostName = LOCAL_HOST_NAME,
                InstanceName = server.InstanceName,
                IsClustered = false,
                np = string.Empty,
                rpc = string.Empty,
                spx = string.Empty,
                tcp = string.Empty,
                Version = ServerVersions.GetKnownVersion(server.Version),
                VersionString = server.Version,
                via = string.Empty
            };
        }
    }
}