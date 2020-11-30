using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Timers;
using System.Text;
using System.Threading;
using Model;
using DataStorage.DataProviders;

namespace Controller
{
	public class TrackingService: ITrackingService
	{
		private ServiceStorage storage = new ServiceStorage();
		public Dictionary<int, bool> statuses = new Dictionary<int, bool>();
		private Dictionary<int, System.Timers.Timer> timers = new Dictionary<int, System.Timers.Timer>();

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

		public void AddService(Service s)
        {
			storage.AddService(s);
        }

		public int AddService(string type, string url, string checkUrl = "api/products/isalive", int timeCheck = 10)
        {
			int i = -1;
			if (type == "WebService")
            {
				i = storage.AddWebService(url, checkUrl, timeCheck);
				WebServiceDataProvider.InsertService(i, (WebService)storage.storage[i]);
				timers[i] = new System.Timers.Timer(timeCheck * 1000);
				timers[i].Elapsed += (source, e) => OnTimedEvent(source, e, i);
				timers[i].Enabled = true;
				timers[i].AutoReset = true;
				statuses[i] = false;
			} else
            {
				i = storage.AddWebService("url");
				timers[i] = new System.Timers.Timer(timeCheck * 1000);
            }
			return i;
        }

		private void OnTimedEvent(Object source, ElapsedEventArgs e, int i)
		{ 
			bool status = storage.storage[i].IsAlive().getStatus();
			lock (statuses)
			{
				statuses[i] = status;
			}
		}

		public void DeleteService(int id)
        {
			timers[id].Enabled = false;
			timers.Remove(id);
			statuses.Remove(id);
			storage.DeleteService(id);
			WebServiceDataProvider.DeleteById(id);
		}

		public List<Tuple<int, WebService>> LoadServicesDB()
        {
			List<Tuple<int, WebService>> ret = new List<Tuple<int, WebService>>();
			var webServices = WebServiceDataProvider.GetAllServices();
			foreach(var service in webServices)
            {
				if (service.Type == "WebService")
				{
					var t = storage.AddWebServiceId(service.Id, service.Url, service.CheckUrl, service.TimeCheck);
					int i = t.Item1;
					var s = t.Item2;
					timers[i] = new System.Timers.Timer(s.timeCheck * 1000);
					timers[i].Elapsed += (source, e) => OnTimedEvent(source, e, i);
					timers[i].Enabled = true;
					timers[i].AutoReset = true;
					statuses[i] = false;
					ret.Add(new Tuple<int, WebService>(i, s));
				}
            }
			return ret;
        }

		public void SaveServicesDB()
        {
			foreach (var item in storage.storage)
			{
                if (item.Value is WebService service)
                {
                    WebServiceDataProvider.InsertService(item.Key, service);
                }
            }
		}
	}
}
