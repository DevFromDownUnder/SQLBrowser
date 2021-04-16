using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SQLBrowser.Example
{
    internal class Program
    {
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

            browser.ReceiveExceptionAction = SQL.Discovery.ExceptionActions.Log;
            browser.SendExceptionAction = SQL.Discovery.ExceptionActions.Log;

            browser.OnSQLServerDiscovered += Browser_OnSQLServerDiscovered;

            Console.WriteLine("Searching for servers...");
            Console.WriteLine();

            try
            {
                await browser.Discover();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine();
            Console.WriteLine("Searching complete");

            Console.ReadLine();
        }

        private static void Browser_OnSQLServerDiscovered(object sender, SQL.Server e)
        {
            Console.WriteLine(e.Response.Responder.ToString() + " - " + e.Response.Response);
        }
    }
}