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
            foreach (var service in Storage)
            {
                Status res = service.Value.IsAlive();
                ret.Add(res);
            }
            return ret;
        }

        public int AddService(Service s)
        {
            Storage[s.Id] = s;
            return s.Id;
        }

        public int AddService(string url)
        {
            WebService service = new WebService(lastId, url);
            Storage[lastId++] = service;
            return lastId - 1;
        }

        public int AddService(string url, string checkUrl, int timeCheck=10)
        {
            WebService service = new WebService(lastId, url, checkUrl, timeCheck);
            Storage[lastId++] = service;
            return lastId - 1;
        }

        public WebService AddServiceId(int Id, string url, string checkUrl, int timeCheck = 10)
        {
            WebService service = new WebService(Id, url, checkUrl, timeCheck);
            try
            {
                Storage[Id] = service;
                lastId = Math.Max(lastId, Id) + 1;
            }
            catch (Exception e)
            {
                Logger.Error("Error occured while adding WebService by Id", e);
                Storage[lastId++] = service;
            }
            return service;
        }

        public void UpdateService(int Id, int timeCheck)
        {
            try
            {
                Service service = Storage[Id];
                service.TimeCheck = timeCheck;
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
                WebService service = (WebService)Storage[Id];
                service.CheckUrl = checkUrl;
                service.TimeCheck = timeCheck;
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
