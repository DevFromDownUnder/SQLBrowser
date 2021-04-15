using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLBrowser.SQL
{
    public class Discovery
    {
        public const int DEFAULT_TCP_PORT = 1433;
        public const int DEFAULT_UDP_PORT = 1434;

        public static byte[] DISCOVERY_PAYLOAD = { 0x02 };

        public enum ExceptionActions
        {
            None,
            Log,
            Throw,
            LogAndThrow
        }
    }
}