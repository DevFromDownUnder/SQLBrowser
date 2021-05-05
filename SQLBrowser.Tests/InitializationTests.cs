using DevFromDownUnder.SQLBrowser;
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
        }
    }
}