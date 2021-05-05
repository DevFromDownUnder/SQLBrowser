using System;
using System.Collections.Generic;

namespace DevFromDownUnder.SQLBrowser.SQL.Network
{
    public class NetworkServers
    {
        public const string DELIMITER = ";;";

        public static List<NetworkServer> Parse(NetworkServerResponse response)
        {
            var result = new List<NetworkServer>();

            if (response.Response.Contains(DELIMITER))
            {
                var tokens = response.Response.Split(DELIMITER, StringSplitOptions.RemoveEmptyEntries);

                foreach (var token in tokens)
                {
                    result.Add(NetworkServer.Parse(response, token));
                }
            }

            return result;
        }
    }
}