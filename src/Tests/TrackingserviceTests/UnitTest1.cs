using Microsoft.VisualStudio.TestTools.UnitTesting;
using Controller;
using System.Collections.Generic;

namespace TrackingserviceTests
{
    [TestClass]
    public class TrackingServiceTests
    {
        private TrackingService t = new TrackingService();

        [TestMethod]
        public void TestCheckServices()
        {
            TestServiceGood good = new TestServiceGood();
            TestServiceBad bad = new TestServiceBad();
            t.AddService(good);
            t.AddService(bad);
            List<bool> res = t.CheckServices();
            Assert.IsTrue(res[0]);
            Assert.IsFalse(res[1]);
        }
    }
}
