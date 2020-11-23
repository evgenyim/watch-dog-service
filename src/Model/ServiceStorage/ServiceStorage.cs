using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
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

        public void DeleteService(int id)
        {
            storage.Remove(id);
        }

        public void LoadServices()
        {
            
        }

    }
}
