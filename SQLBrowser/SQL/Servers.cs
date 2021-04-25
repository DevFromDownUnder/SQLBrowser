using System;
using System.Collections.Generic;

namespace DevFromDownUnder.SQLBrowser.SQL
{
    public class Servers : EventArgs
    {
        public const string DELIMITER = ";;";

        public static List<Server> Parse(ServerResponse response)
        {
            var result = new List<Server>();

            if (response.Response.Contains(DELIMITER))
            {
                var tokens = response.Response.Split(DELIMITER, StringSplitOptions.RemoveEmptyEntries);

                foreach (var token in tokens)
                {
                    result.Add(Server.Parse(response, token));
                }
            }

            return result;
        }
    }
}