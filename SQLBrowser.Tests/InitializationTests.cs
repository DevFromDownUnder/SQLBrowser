using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SQLBrowser.Tests
{
    [TestClass]
    public class InitializationTests
    {
        [TestMethod]
        public void CreateInstance()
        {
            var browser = new Browser();
        }
    }
}