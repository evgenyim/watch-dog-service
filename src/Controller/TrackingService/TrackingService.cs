using System;
using System.Collections.Generic;
using System.Timers;
using Model;
using DataStorage.DataProviders;
using System.IO;
using Newtonsoft.Json;
using Model.Other;

namespace Controller
{
	public class TrackingService : ITrackingService
	{
		private ServiceStorage storage = new ServiceStorage();
		public Dictionary<int, bool> statuses = new Dictionary<int, bool>();
		public Dictionary<int, Denial> denials = new Dictionary<int, Denial>();
		public List<int> newDenials = new List<int>();
		private Dictionary<int, Timer> timers = new Dictionary<int, Timer>();
		private int nextDenialIdx = 0;
		private List<int> returnDenials = new List<int>();
		private int lastCheckedDenials = 0;


		public List<Tuple<int, Status>> CheckServices()
		{
			var ret =  storage.CheckServices();
			foreach (var item in ret)
            {
				int i = item.Item1;
				bool s = item.Item2.getStatus();
				if (statuses.ContainsKey(i) && s != statuses[i])
				{
					denials[nextDenialIdx++] = new Denial(i, s);
					newDenials.Add(nextDenialIdx - 1);
					returnDenials.Add(nextDenialIdx - 1);
				}
				statuses[i] = s;
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
				AddTimer(i, timeCheck);
			}
			else
			{
				i = storage.AddWebService("url");
				timers[i] = new Timer(timeCheck * 1000);
			}
			return i;
		}

		public void UpdateService(int Id, string type, string checkUrl = "api/products/isalive", int timeCheck = 10)
		{
			if (type == "WebService")
			{
				storage.UpdateWebService(Id, checkUrl, timeCheck);
				WebServiceDataProvider.InsertService(Id, (WebService)storage.storage[Id]);
				timers[Id].Interval = timeCheck * 1000;
			}
			else
			{
				storage.UpdateService(Id, timeCheck);
				timers[Id].Interval = timeCheck * 1000;
			}
		}

		public void DeleteService(int id)
		{
			timers[id].Enabled = false;
			timers.Remove(id);
			statuses.Remove(id);
			storage.DeleteService(id);
			WebServiceDataProvider.DeleteById(id);
			DenialDataProvider.DeleteByServiceId(id);
		}

		private void AddTimer(int i, int timeCheck)
		{
			timers[i] = new Timer(timeCheck * 1000);
			timers[i].Elapsed += (source, e) => OnTimedEvent(source, e, i);
			timers[i].Enabled = true;
			timers[i].AutoReset = true;
		}

		private void OnTimedEvent(Object source, ElapsedEventArgs e, int i)
		{
			bool status = storage.storage[i].IsAlive().getStatus();
			lock (statuses)
			{
				if (statuses.ContainsKey(i) && status != statuses[i])
				{
					lock (denials)
					{
						denials[nextDenialIdx++] = new Denial(i, status);
						newDenials.Add(nextDenialIdx - 1);
						returnDenials.Add(nextDenialIdx - 1);
					}
				}
				statuses[i] = status;
			}
		}

		public List<Tuple<int, Denial, string>> GetDenials()
        {
			List<Tuple<int, Denial, string>> ret = new List<Tuple<int, Denial, string>>();
			foreach (int i in returnDenials)
            {
				ret.Add(new Tuple<int, Denial, string>(i, denials[i], storage.storage[denials[i].serviceId].url));
            }
			returnDenials.Clear();
			return ret;
        }

		public void DeleteDenial(int id)
        {
			denials.Remove(id);
			DenialDataProvider.DeleteById(id);
		}

		public List<Tuple<int, Service>> LoadServicesDB()
		{
			List<Tuple<int, Service>> ret = new List<Tuple<int, Service>>();
			var webServices = WebServiceDataProvider.GetAllServices();
			foreach (var service in webServices)
			{
				if (service.Type == "WebService")
				{
					var t = storage.AddWebServiceId(service.Id, service.Url, service.CheckUrl, service.TimeCheck);
					int i = t.Item1;
					var s = t.Item2;
					AddTimer(i, s.timeCheck);
					ret.Add(new Tuple<int, Service>(i, s));
				}
			}
			return ret;
		}

		public List<Tuple<int, Denial, string>> LoadDenialsDB()
        {
			var oldDenials = DenialDataProvider.GetAllDenials();
			List<Tuple<int, Denial, string>> ret = new List<Tuple<int, Denial, string>>();
			foreach (var denial in oldDenials)
			{
				var denial_ = new Denial(denial.ServiceId, denial.StartWorking, denial.Time);
				denials[denial.Id] = denial_;
				nextDenialIdx = Math.Max(nextDenialIdx, denial.Id) + 1;
				ret.Add(new Tuple<int, Denial, string>(denial.Id, denial_, storage.storage[denial_.serviceId].url));
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

		public void SaveDenialsDB()
        {
			foreach (var index in newDenials)
            {
				if (denials.ContainsKey(index))
                {
					DenialDataProvider.InsertDenial(index, denials[index]);
                }
				else
                {
					DenialDataProvider.DeleteById(index);
                }
            }
        }

		public List<Tuple<int, Service>> LoadServicesFile()
		{
			string jsonString;
			try
            {
				jsonString = File.ReadAllText("Services.txt");
				var settings = new JsonSerializerSettings
				{
					TypeNameHandling = TypeNameHandling.All
				};
				storage = (ServiceStorage)JsonConvert.DeserializeObject(jsonString, settings);
				storage.updateLastId();
			}
			catch (Exception e)
            {
				Logger.Error("Error occured in LoadServicesFile", e);
            }
			
			List<Tuple<int, Service>> ret = new List<Tuple<int, Service>>();
			foreach(var item in storage.storage)
			{
				int i = item.Key;
				if (item.Value is WebService service)
				{
					AddTimer(i, service.timeCheck);
					ret.Add(new Tuple<int, Service>(i, service));
				}
			}
			return ret;
		}

		public List<Tuple<int, Denial, string>> LoadDenialsFile()
        {
			string jsonString;
			List<Tuple<int, Denial, string>> ret = new List<Tuple<int, Denial, string>>();
			try
			{
				jsonString = File.ReadAllText("Denials.txt");
				var settings = new JsonSerializerSettings
				{
					TypeNameHandling = TypeNameHandling.All
				};
				denials = (Dictionary<int, Denial>) JsonConvert.DeserializeObject(jsonString, settings);
				foreach (var item in denials)
				{
					Denial denial = item.Value;
					nextDenialIdx = Math.Max(nextDenialIdx, item.Key) + 1;
					ret.Add(new Tuple<int, Denial, string>(item.Key, denial, storage.storage[denial.serviceId].url));
				}
			}
			catch (Exception e)
			{
				Logger.Error("Error occured in LoadServicesFile", e);
			}
			return ret;
		}

		public void SaveServicesFile()
		{
			var settings = new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.All
			};
			string s = JsonConvert.SerializeObject(storage, settings);
			File.WriteAllText("Services.txt", s);
		}

		public void SaveDenialsFile()
        {
			var settings = new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.All
			};
			string s = JsonConvert.SerializeObject(denials, settings);
			File.WriteAllText("Denials.txt", s);
		}

	}
}
