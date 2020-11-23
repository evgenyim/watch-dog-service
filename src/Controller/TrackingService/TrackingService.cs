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
	public class TrackingService: ITrackingService
	{
		private ServiceStorage storage = new ServiceStorage();

		public List<Tuple<int, Status>> CheckServices()
        {
			List<Tuple<int, Status>> ret = new List<Tuple<int, Status>>();
			foreach(var s in storage.storage)
            {
				Status res = s.Value.IsAlive();
				ret.Add(new Tuple<int, Status>(s.Key, res)); 
            }
			return ret;
		}

		public int AddWebservice(string url, string checkUrl="api/products/isalive", int timeCheck=10)
        {
			int i;
			bool bNum = int.TryParse(url, out i);
			if (bNum)
				i = storage.AddWebService("http://localhost:" + url + "/", checkUrl, timeCheck);
			else
				i = storage.AddWebService(url, checkUrl, timeCheck);
			return i;
		}

		public void AddService(Service s)
        {
			storage.AddService(s);
        }

		public void DeleteService(int id)
        {
			storage.DeleteService(id);
        }
	}
}
