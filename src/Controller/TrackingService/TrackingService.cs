using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;

using System.Text;
using System.Threading;
using Model;

namespace Controller
{
	public class TrackingService
	{
		ServiceStorage storage = new ServiceStorage();
		public TrackingService()
		{

		}

		public List<bool> CheckServices()
        {
			List<bool> ret = new List<bool>();
			foreach(Service s in storage.storage)
            {
				bool res = s.IsAlive();
				ret.Add(res); 
            }
			return ret;
		}

		public void AddWebservice(string port)
        {
			storage.AddWebService("http://localhost:" + port + "/");
        }

		public void AddService(Service s)
        {
			storage.storage.Add(s);
        }
	}
}
