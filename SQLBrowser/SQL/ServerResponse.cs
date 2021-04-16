using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SQLBrowser.SQL
{
    public class ServerResponse
    {
        public const byte RESPONSE_ID_BYTE = 0x05;

        public IPEndPoint Responder { get; set; }

        public byte Id { get; set; }

        public ushort Size { get; set; }

        public byte[] Data { get; set; }

        public string Response { get; set; }

        public static ServerResponse Parse(byte[] buffer, IPEndPoint responder)
        {
            if (buffer == null)
            {
                return null;
            }

            if (buffer.Length < 3)
            {
                return null;
            }

            if (buffer[0] != RESPONSE_ID_BYTE)
            {
                return null;
            }

            return new ServerResponse()
            {
                Data = buffer,
                Id = buffer[0],
                Response = Encoding.ASCII.GetString(buffer, 3, buffer.Length - 3),
                Responder = responder,
                Size = BitConverter.ToUInt16(buffer, 1)
            };
        }
    }
}