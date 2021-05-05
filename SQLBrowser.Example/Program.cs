using DevFromDownUnder.SQLBrowser;
using DevFromDownUnder.SQLBrowser.SQL;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading.Tasks;

namespace SQLBrowser.Example
{
    internal class Program
    {
        [SupportedOSPlatform("windows")]
        private static async Task Main(string[] args)
        {
            using ILoggerFactory loggerFactory =
                LoggerFactory.Create(builder =>
                    builder.AddSimpleConsole(options =>
                    {
                        options.IncludeScopes = true;
                        options.SingleLine = true;
                        options.TimestampFormat = "hh:mm:ss ";
                    }));

            var browser = new Browser(loggerFactory.CreateLogger<Program>());

            browser.OnServerDiscovered += Browser_OnServerDiscovered;

            Console.WriteLine("Searching for servers...");
            Console.WriteLine();

            try
            {
                var result = await browser.DiscoverServers();

                Console.WriteLine();

                Console.WriteLine("[{0}]", string.Join(", ", result.OrderBy((x) => x.ToString()).Select((x) => $"[{x.ServerName}, {x.Version}]")));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine();
            Console.WriteLine("Searching complete");

            Console.ReadLine();
        }

        private static void Browser_OnServerDiscovered(object sender, Server e)
        {
            //Console.WriteLine(e.ServerName);
        }
    }
}