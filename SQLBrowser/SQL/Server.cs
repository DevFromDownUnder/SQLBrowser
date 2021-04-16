using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLBrowser.SQL
{
    public class Server : EventArgs
    {
        public ServerResponse Response { get; set; }

        public static List<Server> Parse(ServerResponse response)
        {
            var servers = new List<Server>();

            //Pass
            servers.Add(new Server() { Response = response });

            return servers;
        }
    }
}