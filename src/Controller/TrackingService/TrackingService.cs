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

namespace Controller.TrackingService
{
	public class TrackingService : ITrackingService
	{
		internal ServiceStorage storage = new ServiceStorage();
		public ConcurrentDictionary<int, bool> Statuses = new ConcurrentDictionary<int, bool>();
		public ConcurrentDictionary<int, Denial> Denials = new ConcurrentDictionary<int, Denial>();
		private Dictionary<int, Timer> timers = new Dictionary<int, Timer>();
		internal int nextDenialIdx = 0;
		private List<int> returnDenials = new List<int>();
		private Loader loader;

		public TrackingService()
        {
			loader = new Loader(this);
		}


		public List<Status> CheckServices()
		{
			var ret = storage.CheckServices();
			foreach (var status in ret)
			{
				int i = status.ServiceId;
				bool s = status.IsAlive;
				if (Statuses.ContainsKey(i) && s != Statuses[i])
				{
					Denials[nextDenialIdx++] = new Denial(i, s);
					returnDenials.Add(nextDenialIdx - 1);
				}
				Statuses[i] = s;
			}
			return ret;
		}

		public void AddService(Service s)
		{
			int i = storage.AddService(s);
			AddTimer(i, s.TimeCheck);
		}

		public int AddService(string type, string url, string checkUrl, int timeCheck = 10)
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

		public void UpdateService(int Id, string type, string checkUrl, int timeCheck = 10)
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
			DenialDataProvider.DeleteByServiceId(id);
			foreach (var item in Denials)
            {
				if (item.Value.ServiceId == id)
                {
					Denials.TryRemove(item.Key, out Denial _);
                }
            }
			WebServiceDataProvider.DeleteById(id);
		}

		internal void AddTimer(int i, int timeCheck)
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
            return loader.LoadServices(fromDB);
        }

        public List<IndexedDenial> LoadDenials(bool fromDB)
        {
			return loader.LoadDenials(fromDB);
        }

        public void Save(bool toDB)
        {
            loader.Save(toDB);
        }
    }
}
