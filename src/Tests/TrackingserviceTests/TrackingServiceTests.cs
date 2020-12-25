using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Controller.TrackingService;
using Model.ServiceStorage;
using System.Threading;

namespace TrackingserviceTests
{
    [TestClass]
    public class TrackingServiceTests
    {
        private TrackingService trackingService = new TrackingService();

        [TestMethod]
        public void TestCheckServices()
        {
            TestServiceGood good = new TestServiceGood();
            TestServiceBad bad = new TestServiceBad();
            trackingService.AddService(good);
            trackingService.AddService(bad);
            List<Status> res = trackingService.CheckServices();
            Assert.IsTrue(res[0].IsAlive);
            Assert.IsFalse(res[1].IsAlive);
        }

        [TestMethod]
        public void TestDenialsBecomeWorking()
        {
            TestServiceChanging changing = new TestServiceChanging();
            trackingService.AddService(changing);
            List<Status> res1 = trackingService.CheckServices();
            List<Status> res2 = trackingService.CheckServices();
            Assert.IsFalse(res1[0].IsAlive);
            Assert.IsTrue(res2[0].IsAlive);
            List<IndexedDenial> denials = trackingService.GetDenials();
            Assert.AreEqual(denials.Count, 1);
            Assert.IsTrue(denials[0].Denial.StartWorking);
        }

        [TestMethod]
        public void TestDenialsBecomeNotWorking()
        {
            TestServiceChanging changing = new TestServiceChanging();
            trackingService.AddService(changing);
            List<Status> res1 = trackingService.CheckServices();
            List<Status> res2 = trackingService.CheckServices();
            List<Status> res3 = trackingService.CheckServices();
            Assert.IsFalse(res1[0].IsAlive);
            Assert.IsTrue(res2[0].IsAlive);
            Assert.IsFalse(res3[0].IsAlive);
            List<IndexedDenial> denials = trackingService.GetDenials();
            Assert.AreEqual(denials.Count, 2);
            Assert.IsFalse(denials[1].Denial.StartWorking);
        }

        [TestMethod]
        public void TestTimers()
        {
            TestServiceChanging changing = new TestServiceChanging();
            trackingService.AddService(changing);
            trackingService.CheckServices();
            var status1 = trackingService.Statuses[3];
            Assert.IsFalse(status1);
            Thread.Sleep(2500);
            var status2 = trackingService.Statuses[3];
            Assert.IsTrue(status2);
            Thread.Sleep(2000);
            var status3 = trackingService.Statuses[3];
            Assert.IsFalse(status3);
        }

    }
}
