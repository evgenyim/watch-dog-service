using System;
using System.Collections.Generic;
using System.Linq;
using Model.ServiceStorage.Interfaces;
using Model.Other;


namespace Model.ServiceStorage
{
    [Serializable]
    public class ServiceStorage: IServiceStorage
    {
        public Dictionary<int, Service> Storage = new Dictionary<int, Service>();
        private int lastId = 0;

        public List<Status> CheckServices()
        {
            List<Status> ret = new List<Status>();
            foreach (var s in Storage)
            {
                Status res = s.Value.IsAlive();
                ret.Add(res);
            }
            return ret;
        }

        public int AddService(Service s)
        {
            Storage[lastId++] = s;
            return lastId - 1;
        }

        public int AddService(string url)
        {
            WebService s = new WebService(lastId, url);
            Storage[lastId++] = s;
            return lastId - 1;
        }

        public int AddService(string url, string checkUrl="api/products/isalive", int timeCheck=10)
        {
            if (checkUrl == "")
            {
                checkUrl = "api/products/isalive";
            }
            WebService s = new WebService(lastId, url, checkUrl, timeCheck);
            Storage[lastId++] = s;
            return lastId - 1;
        }

        public WebService AddServiceId(int Id, string url, string checkUrl = "api/products/isalive", int timeCheck = 10)
        {
            if (checkUrl == "")
            {
                checkUrl = "api/products/isalive";
            }
            WebService s = new WebService(Id, url, checkUrl, timeCheck);
            try
            {
                Storage[Id] = s;
                lastId = Math.Max(lastId, Id) + 1;
            }
            catch (Exception e)
            {
                Logger.Error("Error occured while adding WebService by Id", e);
                Storage[lastId++] = s;
            }
            return s;
        }

        public void UpdateService(int Id, int timeCheck)
        {
            try
            {
                Service s = Storage[Id];
                s.TimeCheck = timeCheck;
            }
            catch (Exception e)
            {
                Logger.Error("Error occured while updating Service", e);
            }
        }

        public void UpdateService(int Id, string checkUrl, int timeCheck)
        {
            try
            {
                WebService s = (WebService)Storage[Id];
                s.CheckUrl = checkUrl;
                s.TimeCheck = timeCheck;
            }
            catch (Exception e)
            {
                Logger.Error("Error occured while updating WebService", e);
            }
        }

        public void DeleteService(int id)
        {
            Storage.Remove(id);
        }

        public void UpdateLastId()
        {
            lastId = Storage.Keys.Max() + 1;
        }
    }
}
