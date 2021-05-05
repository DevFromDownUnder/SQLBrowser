using System.Collections.Generic;
using System.Linq;

namespace DevFromDownUnder.SQLBrowser.SQL
{
    /// <summary>
    /// Data pulled from https://sqlserverbuilds.blogspot.com/
    /// </summary>
    public class ServerVersions
    {
        public enum Version
        {
            Unknown,
            SQLServer2000,
            SQLServer2005,
            SQLServer2008,
            SQLServer2008R2,
            SQLServer2012,
            SQLServer2014,
            SQLServer2016,
            SQLServer2017,
            SQLServer2019,
            SQLServer2021
        }

#pragma warning disable CA2211 // Non-constant fields should not be visible

        public static List<(Version, string)> MajorMinorVersions = new()
#pragma warning restore CA2211 // Non-constant fields should not be visible
        {
            (Version.SQLServer2000, "8.0"),
            (Version.SQLServer2005, "9.0"),
            (Version.SQLServer2008, "10.0"),
            (Version.SQLServer2008R2, "10.50"),
            (Version.SQLServer2012, "11.0"),
            (Version.SQLServer2014, "12.0"),
            (Version.SQLServer2016, "13.0"),
            (Version.SQLServer2017, "14.0"),
            (Version.SQLServer2019, "15.0"),
            (Version.SQLServer2021, "16.0")
        };

        public static Version GetKnownVersion(string version)
        {
            var found = MajorMinorVersions.FirstOrDefault((v) => version?.StartsWith(v.Item2) ?? false);

            return (found == default) ? Version.Unknown : found.Item1;
        }
    }
}