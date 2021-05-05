namespace DevFromDownUnder.SQLBrowser.SQL.Network
{
    public class NetworkConfiguration
    {
        public const int DEFAULT_TIMEOUT = 2000;
        public const int DEFAULT_RESEND_DELAY = 500;

        public int Port { get; set; } = Architecture.DEFAULT_UDP_PORT;
        public int Timeout { get; set; } = DEFAULT_TIMEOUT;
        public int ResendDelay { get; set; } = DEFAULT_RESEND_DELAY;
        public Architecture.ExceptionActions SendExceptionAction { get; set; } = Architecture.ExceptionActions.Log;
        public Architecture.ExceptionActions ReceiveExceptionAction { get; set; } = Architecture.ExceptionActions.Log;
    }
}