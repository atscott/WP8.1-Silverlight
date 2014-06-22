using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using SmallShopsUnitedDomainLayer;

namespace WebScraper.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            var result = await SmallShopsMerchantsScraper.GetMerchants();
            Assert.IsTrue(((IList<Merchant>)result).Count > 50);
        }
    }
}
