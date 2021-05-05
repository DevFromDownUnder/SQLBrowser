using System;
using System.Net;
using System.Text;

namespace DevFromDownUnder.SQLBrowser.SQL.Network
{
    /// <summary>
    /// See https://docs.microsoft.com/en-us/openspecs/windows_protocols/mc-sqlr/1ea6e25f-bff9-4364-ba21-5dc449a601b7
    /// </summary>
    public class NetworkServerResponse
    {
        public const byte RESPONSE_ID_BYTE = 0x05;

        public IPEndPoint Responder { get; set; }

        public byte Id { get; set; }

        public ushort Size { get; set; }

        public byte[] Data { get; set; }

        public string Response { get; set; }

        public static NetworkServerResponse Parse(byte[] buffer, IPEndPoint responder)
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

            return new NetworkServerResponse()
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