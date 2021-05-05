using System.Runtime.CompilerServices;

namespace DevFromDownUnder.SQLBrowser.Extensions
{
    public static class LoggingExtensions
    {
        public static string CurrentFunction([CallerMemberName] string name = "") => name;
    }
}