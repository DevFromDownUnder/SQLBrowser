using System;

namespace SQLBrowser.Example
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var browser = new Browser() { Timeout = 30000 };

            browser.OnSQLServerDiscovered += Browser_OnSQLServerDiscovered;

            Console.WriteLine("Searching for servers...");

            browser.Discover().Wait();

            Console.ReadLine();
        }

        private static void Browser_OnSQLServerDiscovered(object sender, SQL.Server e)
        {
            Console.WriteLine(e.EndpointString);
            Console.WriteLine(e.RawData);
        }
    }
}