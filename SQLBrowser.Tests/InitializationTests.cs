using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SQLBrowser.Tests
{
    [TestClass]
    public class InitializationTests
    {
        [TestMethod]
        public void CreateInstances()
        {
            Browser browser;

            browser = new();
            browser = new(SQL.Discovery.DEFAULT_UDP_PORT);
        }
    }
}