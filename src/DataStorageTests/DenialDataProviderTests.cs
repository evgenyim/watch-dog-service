using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.DataStorage.DataProviders;
using Model.ServiceStorage;

namespace DataStorageTests
{
    [TestClass]
    public class DenialDataProviderTests
    {
        [TestMethod]
        public void GetAllDenialsTest()
        {
            var res = DenialDataProvider.GetAllDenials();
            Assert.IsTrue(res.Count > 0);
        }

        [TestMethod]
        public void InsertDenialTest()
        {
            WebServiceDataProvider.InsertService(new WebService(9999998, "Test"));
            DenialDataProvider.InsertDenial(9999999, new Denial(9999998, true));
            var res = DenialDataProvider.GetAllDenials();
            bool contains = false;
            foreach (var denial in res)
            {
                if (denial.Id == 9999999)
                    contains = true;
            }
            Assert.IsTrue(contains);
        }

        [TestMethod]
        public void DeleteDenialId()
        {
            DenialDataProvider.DeleteById(9999999);
            var res = DenialDataProvider.GetAllDenials();
            bool contains = false;
            foreach (var denial in res)
            {
                if (denial.Id == 9999999)
                    contains = true;
            }
            Assert.IsFalse(contains);
        }

        [TestMethod]
        public void DeleteDenialServiceId()
        {
            DenialDataProvider.InsertDenial(9999999, new Denial(9999998, true));
            DenialDataProvider.DeleteByServiceId(9999998);
            var res = DenialDataProvider.GetAllDenials();
            bool contains = false;
            foreach (var denial in res)
            {
                if (denial.Id == 9999999)
                    contains = true;
            }
            Assert.IsFalse(contains);
            WebServiceDataProvider.DeleteById(9999998);
        }

    }
}
