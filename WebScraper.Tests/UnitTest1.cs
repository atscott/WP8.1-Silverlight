using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using SmallShopsUnitedScraper;

namespace WebScraper.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var result = SmallShopsMerchantsScraper.GetMerchants();
            var something = result.Result;

            something.ToString();
        }
    }
}
