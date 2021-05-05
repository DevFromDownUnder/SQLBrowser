using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Runtime.Versioning;

namespace DevFromDownUnder.SQLBrowser.SQL.Local
{
    [SupportedOSPlatform("windows")]
    public class LocalServers
    {
        public const string REG_INSTANCES_KEY = "InstalledInstances";

        public static (RegistryKey, string[])[] LookupRegistryInstances()
        {
            var registryInstances = new List<(RegistryKey, string[])>();

            if (Environment.Is64BitOperatingSystem)
            {
                using (var hive32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                {
                    var instances = LookupInstances(hive32);

                    if (instances?.Length > 0)
                    {
                        registryInstances.Add((hive32, instances));
                    }
                }

                using (var hive64 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                {
                    var instances = LookupInstances(hive64);

                    if (instances?.Length > 0)
                    {
                        registryInstances.Add((hive64, instances));
                    }
                }
            }
            else
            {
                using var hive = Registry.LocalMachine;

                var instances = LookupInstances(hive);

                if (instances?.Length > 0)
                {
                    registryInstances.Add((hive, instances));
                }
            }

            return registryInstances.ToArray();
        }

        private static string[] LookupInstances(RegistryKey hive)
        {
            using var key = hive.OpenSubKey($"{LocalServer.REG_SERVER_ROOT_PATH}");

            return key?.GetValue($"{REG_INSTANCES_KEY}") as string[];
        }
    }
}