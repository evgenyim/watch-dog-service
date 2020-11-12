using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public class ServiceStorage: IServiceStorage
    {
        public List<Service> storage = new List<Service>();

        public void AddWebService(string url)
        {
            WebService s = new WebService(url);
            this.storage.Add(s);
        }

        public void DeleteService()
        {

        }

        public void LoadServices()
        {

        }

    }
}
