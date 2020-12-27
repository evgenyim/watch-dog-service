using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.DataStorage.DataProviders;
using Model.ServiceStorage;

namespace DataStorageTests
{
    [TestClass]
    public class WebServiceDataProviderTests
    {
        [TestMethod]
        public void GetAllServicesTest()
        {
            var res = WebServiceDataProvider.GetAllServices();
            Assert.IsTrue(res.Count > 0);
        }

        [TestMethod]
        public void InsertServiceTest()
        {
            WebServiceDataProvider.InsertService(new WebService(9999998, "Test"));
            var res = WebServiceDataProvider.GetAllServices();
            bool containsId = false;
            bool containsUrl = false;
            foreach (var service in res)
            {
                if (service.Id == 9999998)
                    containsId = true;
                if (service.Url == "Test")
                    containsUrl = true;
            }
            Assert.IsTrue(containsId);
            Assert.IsTrue(containsUrl);
        }

        [TestMethod]
        public void DeleteById()
        {
            WebServiceDataProvider.DeleteById(9999998);
            var res = WebServiceDataProvider.GetAllServices();
            bool contains = false;
            foreach (var service in res)
            {
                if (service.Id == 99999998)
                    contains = true;
            }
            Assert.IsFalse(contains);
        }

    }
}
