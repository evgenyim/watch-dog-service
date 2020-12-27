using Model.DataStorage.DataProviders;
using Model.Other;
using Model.ServiceStorage;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controller.TrackingService
{
    public class Loader
    {
		private TrackingService trackingService;

		public Loader(TrackingService t)
        {
			trackingService = t;
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
						var addedService = trackingService.storage.AddServiceId(service.Id, service.Url, service.CheckUrl, service.TimeCheck);
						trackingService.AddTimer(service.Id, service.TimeCheck);
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
					trackingService.storage = (ServiceStorage)JsonConvert.DeserializeObject(jsonString, settings);
					trackingService.storage.UpdateLastId();
				}
				catch (Exception e)
				{
					Logger.Error("Error occured in LoadServicesFile", e);
				}

				foreach (var item in trackingService.storage.Storage)
				{
					int i = item.Key;
					if (item.Value is WebService service)
					{
						trackingService.AddTimer(i, service.TimeCheck);
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
					trackingService.Denials[denial.Id] = denial_;
					trackingService.nextDenialIdx = Math.Max(trackingService.nextDenialIdx, denial.Id) + 1;
					ret.Add(new IndexedDenial(denial.Id, denial_, trackingService.storage.Storage[denial_.ServiceId].Url));
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
					trackingService.Denials = (ConcurrentDictionary<int, Denial>)JsonConvert.DeserializeObject(jsonString, settings);
					foreach (var item in trackingService.Denials)
					{
						Denial denial = item.Value;
						trackingService.nextDenialIdx = Math.Max(trackingService.nextDenialIdx, item.Key) + 1;
						ret.Add(new IndexedDenial(item.Key, denial, trackingService.storage.Storage[denial.ServiceId].Url));
					}
				}
				catch (Exception e)
				{
					Logger.Error("Error occured in LoadDenialsFile", e);
				}
			}
			return ret;
		}

		public void Save(bool toDB)
        {
			SaveServices(toDB);
			SaveDenials(toDB);
        }

		public void SaveServices(bool toDB)
		{
			if (toDB)
			{
				foreach (var item in trackingService.storage.Storage)
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
				string s = JsonConvert.SerializeObject(trackingService.storage, settings);
				File.WriteAllText(Properties.Settings.Default.ServicesFileName, s);
			}
		}

		public void SaveDenials(bool toDB)
		{
			if (toDB)
			{
				foreach (var item in trackingService.Denials)
				{
					DenialDataProvider.InsertDenial(item.Key, item.Value);
				}
			}
			else
			{
				var settings = new JsonSerializerSettings
				{
					TypeNameHandling = TypeNameHandling.All
				};
				string s = JsonConvert.SerializeObject(trackingService.Denials, settings);
				File.WriteAllText(Properties.Settings.Default.DenialsFileName, s);
			}
		}
	}
}
