using System;
using System.Collections.Generic;
using System.Timers;
using Model.ServiceStorage;
using Model.DataStorage.DataProviders;
using System.IO;
using Newtonsoft.Json;
using Model.Other;
using System.Collections.Concurrent;
using Controller.Interfaces;
using ModelDataStorage.DataProviders;

namespace Controller.TrackingService
{
	public class TrackingService : ITrackingService
	{
		private ServiceStorage storage = new ServiceStorage();
		public ConcurrentDictionary<int, bool> Statuses = new ConcurrentDictionary<int, bool>();
		public ConcurrentDictionary<int, Denial> Denials = new ConcurrentDictionary<int, Denial>();
		public List<int> NewDenials = new List<int>();
		private Dictionary<int, Timer> timers = new Dictionary<int, Timer>();
		private int nextDenialIdx = 0;
		private List<int> returnDenials = new List<int>();


		public List<Status> CheckServices()
		{
			var ret =  storage.CheckServices();
			foreach (var status in ret)
            {
				int i = status.ServiceId;
				bool s = status.IsAlive;
				if (Statuses.ContainsKey(i) && s != Statuses[i])
				{
					Denials[nextDenialIdx++] = new Denial(i, s);
					NewDenials.Add(nextDenialIdx - 1);
					returnDenials.Add(nextDenialIdx - 1);
				}
				Statuses[i] = s;
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
				i = storage.AddService(url, checkUrl, timeCheck);
				WebServiceDataProvider.InsertService((WebService)storage.Storage[i]);
				AddTimer(i, timeCheck);
			}
			else
			{
				i = storage.AddService("url");
				timers[i] = new Timer(timeCheck * 1000);
			}
			return i;
		}

		public void UpdateService(int Id, string type, string checkUrl = "api/products/isalive", int timeCheck = 10)
		{
			if (type == "WebService")
			{
				storage.UpdateService(Id, checkUrl, timeCheck);
				WebServiceDataProvider.InsertService((WebService)storage.Storage[Id]);
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
			Statuses.TryRemove(id, out bool _);
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
			bool status = storage.Storage[i].IsAlive().IsAlive;
			if (Statuses.ContainsKey(i) && status != Statuses[i])
			{
				Denials[nextDenialIdx++] = new Denial(i, status);
				NewDenials.Add(nextDenialIdx - 1);
				returnDenials.Add(nextDenialIdx - 1);
			}
			Statuses[i] = status;
		}

		public List<IndexedDenial> GetDenials()
        {
			List<IndexedDenial> ret = new List<IndexedDenial>();
			foreach (int i in returnDenials)
            {
				ret.Add(new IndexedDenial(i, Denials[i], storage.Storage[Denials[i].ServiceId].Url));
            }
			returnDenials.Clear();
			return ret;
        }

		public void DeleteDenial(int id)
        {
			Denials.TryRemove(id, out Denial _);
			DenialDataProvider.DeleteById(id);
		}

		public List<Service> LoadServices(bool fromDB)
		{
			List<Service> ret = new List<Service>();
			if (fromDB)
			{
				var webServices = WebServiceDataProvider.GetAllServices();
				foreach (var service in webServices)
				{
					if (service.Type == "WebService")
					{
						var addedService = storage.AddServiceId(service.Id, service.Url, service.CheckUrl, service.TimeCheck);
						AddTimer(service.Id, service.TimeCheck);
						ret.Add(addedService);
					}
				}
			}
            else
            {
				string jsonString;
				try
				{
					jsonString = File.ReadAllText(Properties.Settings.Default.ServicesFileName);
					var settings = new JsonSerializerSettings
					{
						TypeNameHandling = TypeNameHandling.All
					};
					storage = (ServiceStorage)JsonConvert.DeserializeObject(jsonString, settings);
					storage.UpdateLastId();
				}
				catch (Exception e)
				{
					Logger.Error("Error occured in LoadServicesFile", e);
				}

				foreach (var item in storage.Storage)
				{
					int i = item.Key;
					if (item.Value is WebService service)
					{
						AddTimer(i, service.TimeCheck);
						ret.Add(service);
					}
				}
			}
			return ret;
		}

		public List<IndexedDenial> LoadDenials(bool fromDB)
        {
			List<IndexedDenial> ret = new List<IndexedDenial>();
			if (fromDB)
			{
				var oldDenials = DenialDataProvider.GetAllDenials();
				foreach (var denial in oldDenials)
				{
					var denial_ = new Denial(denial.ServiceId, denial.StartWorking, denial.Time);
					Denials[denial.Id] = denial_;
					nextDenialIdx = Math.Max(nextDenialIdx, denial.Id) + 1;
					ret.Add(new IndexedDenial(denial.Id, denial_, storage.Storage[denial_.ServiceId].Url));
				}
			}
			else
            {
				string jsonString;
				try
				{
					jsonString = File.ReadAllText(Properties.Settings.Default.DenialsFileName);
					var settings = new JsonSerializerSettings
					{
						TypeNameHandling = TypeNameHandling.All
					};
					Denials = (ConcurrentDictionary<int, Denial>)JsonConvert.DeserializeObject(jsonString, settings);
					foreach (var item in Denials)
					{
						Denial denial = item.Value;
						nextDenialIdx = Math.Max(nextDenialIdx, item.Key) + 1;
						ret.Add(new IndexedDenial(item.Key, denial, storage.Storage[denial.ServiceId].Url));
					}
				}
				catch (Exception e)
				{
					Logger.Error("Error occured in LoadDenialsFile", e);
				}
			}
			return ret;
		}

		public void SaveServices(bool toDB)
		{
			if (toDB)
			{
				foreach (var item in storage.Storage)
				{
					if (item.Value is WebService service)
					{
						WebServiceDataProvider.InsertService(service);
					}
				}
			}
			else
            {
				var settings = new JsonSerializerSettings
				{
					TypeNameHandling = TypeNameHandling.All
				};
				string s = JsonConvert.SerializeObject(storage, settings);
				File.WriteAllText(Properties.Settings.Default.ServicesFileName, s);
			}
		}

		public void SaveDenials(bool toDB)
        {
			if (toDB)
			{
				foreach (var index in NewDenials)
				{
					if (Denials.ContainsKey(index))
					{
						DenialDataProvider.InsertDenial(index, Denials[index]);
					}
					else
					{
						DenialDataProvider.DeleteById(index);
					}
				}
			}
			else
            {
				var settings = new JsonSerializerSettings
				{
					TypeNameHandling = TypeNameHandling.All
				};
				string s = JsonConvert.SerializeObject(Denials, settings);
				File.WriteAllText(Properties.Settings.Default.ServicesFileName, s);
			}
        }
	}
}
