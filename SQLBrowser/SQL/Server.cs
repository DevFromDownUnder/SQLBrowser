using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLBrowser.SQL
{
    public class Server : EventArgs
    {
        public string RawData { get; set; }
        public string EndpointString { get; set; }

        public static Server ParseServer(string endpointString, string data)
        {
            var server = new Server();

            if (string.IsNullOrWhiteSpace(data)) return null;

            server.EndpointString = endpointString;
            server.RawData = data;

            return server;
        }
    }
}