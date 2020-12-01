using System;
using System.Collections.Generic;
using System.Linq;
using Model.Other;


namespace Model
{
    [Serializable]
    public class ServiceStorage: IServiceStorage
    {
        public Dictionary<int, Service> storage = new Dictionary<int, Service>();
        private int lastId = 0;

        public int AddService(Service s)
        {
            storage[lastId++] = s;
            return lastId - 1;
        }

        public int AddWebService(string url)
        {
            WebService s = new WebService(url);
            storage[lastId++] = s;
            return lastId - 1;
        }

        public int AddWebService(string url, string checkUrl="api/products/isalive", int timeCheck=10)
        {
            if (checkUrl == "")
            {
                checkUrl = "api/products/isalive";
            }
            WebService s = new WebService(url, checkUrl, timeCheck);
            storage[lastId++] = s;
            return lastId - 1;
        }

        public Tuple<int, WebService> AddWebServiceId(int Id, string url, string checkUrl = "api/products/isalive", int timeCheck = 10)
        {
            if (checkUrl == "")
            {
                checkUrl = "api/products/isalive";
            }
            WebService s = new WebService(url, checkUrl, timeCheck);
            try
            {
                storage[Id] = s;
                lastId = Math.Max(lastId, Id) + 1;
            }
            catch (Exception e)
            {
                Logger.Error("Error occured while adding WebService by Id", e);
                storage[lastId++] = s;
            }
            return new Tuple<int, WebService>(lastId - 1, s);
        }

        public void DeleteService(int id)
        {
            storage.Remove(id);
        }

        public void updateLastId()
        {
            lastId = storage.Keys.Max() + 1;
        }
    }
}
