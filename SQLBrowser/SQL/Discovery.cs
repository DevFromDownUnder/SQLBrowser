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

        public static byte[] CLNT_BCAST_EX = { 0x02 };
        public static byte[] CLNT_UCAST_EX = { 0x03 };
        public static byte[] CLNT_UCAST_INST = { 0x04 };
        public static byte[] CLNT_UCAST_DAC = { 0x0F };

        public enum ExceptionActions
        {
            None,
            Log,
            Throw,
            LogAndThrow
        }
    }
}