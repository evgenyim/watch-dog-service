using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Timers;
using System.Text;
using System.Threading;
using Model;

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

		public int AddWebservice(string url, string checkUrl="api/products/isalive", int timeCheck=10)
        {
			int i;
			bool bNum = int.TryParse(url, out i);
			if (bNum)
				i = storage.AddWebService("http://localhost:" + url + "/", checkUrl);
			else
				i = storage.AddWebService(url, checkUrl);
			return i;
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
				i = storage.AddWebService(url, checkUrl);
				timers[i] = new System.Timers.Timer(timeCheck * 1000);
				timers[i].Elapsed += (source, e) => OnTimedEvent(source, e, i);
				timers[i].Enabled = true;
				timers[i].AutoReset = true;
				statuses[i] = false;
			}
			return i;
        }

		private void OnTimedEvent(Object source, ElapsedEventArgs e, int i)
		{ 
			lock (statuses)
			{
				statuses[i] = storage.storage[i].IsAlive().getStatus();
			}
			Console.WriteLine("HERR " + statuses[i].ToString());
		}

		public void DeleteService(int id)
        {
			timers[id].Enabled = false;
			timers.Remove(id);
			storage.DeleteService(id);
			statuses.Remove(id);
		}
	}
}
