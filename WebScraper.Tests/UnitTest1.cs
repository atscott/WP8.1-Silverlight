using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using SmallShopsUnitedScraper;

namespace WebScraper.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            var result = await SmallShopsMerchantsScraper.GetMerchants();
            Assert.IsTrue(result.Count > 50);
        }
    }
}
