using Controller.TrackingService;
using GUI.Model;
using GUI.ViewModel.Commands;
using Model.Other;
using Model.ServiceStorage;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace GUI.ViewModel
{
	public class ApplicationViewModel : INotifyPropertyChanged
	{
		private ConcurrentDictionary<int, bool> services = new ConcurrentDictionary<int, bool>();

		private TrackingService trackingService = new TrackingService();
		private Thread thread;
		private bool loadFromDb;
        public Window Window;

        public ObservableCollection<ServiceItem> Panel { get; set; } = new ObservableCollection<ServiceItem>();
        public ObservableCollection<DenialItem> DenialsPanel { get; set; } = new ObservableCollection<DenialItem>();


        #region Commands

        private RelayCommand addCommand;
        public RelayCommand AddCommand
        {
            get
            {
                return addCommand ??
                       (addCommand = new RelayCommand(obj =>
                       {
                           AddForm f = new AddForm(this);
                           f.Show();
                       }));
            }
        }

        private RelayCommand checkCommand;
        public RelayCommand CheckCommand
        {
            get
            {
                return checkCommand ??
                       (checkCommand = new RelayCommand(obj =>
                       {
                           Task.Run(() =>
                           {
                               List<Status> l = trackingService.CheckServices();
                               foreach (var s in l)
                               {
                                   services[s.ServiceId] = s.IsAlive;
                               }
                               SetStatuses();
                               MessageBox.Show("Services checked!");
                           });
                       }));
            }
        }

        #endregion

        public bool FromDB
        {
            get => loadFromDb;
            set
            {
                loadFromDb = value;
            }
        }

        public bool FromFile
        {
            get => !loadFromDb;
            set
            {
                loadFromDb = !value;
            }
        }
        public ApplicationViewModel(Window window)
		{
            Window = window;
            loadFromDb = Properties.Settings.Default.loadFromDB;

            ServiceItem.parent = this;
            DenialItem.parent = this;

            List<Service> list;
            List<IndexedDenial> denials;
            list = trackingService.LoadServices(loadFromDb);
            denials = trackingService.LoadDenials(loadFromDb);
            foreach (var service_ in list)
            {
                if (service_ is WebService service)
                {
                    ServiceItem item = new ServiceItem
                    {
                        Id = service.Id,
                        Type = "WebService",
                        Name = service.Url,
                        CheckUrl = service.CheckUrl,
                        TimeCheck = service.TimeCheck,
                        IsAlive = false
                    };
                    Panel.Add(item);
                }
            }
            foreach (var indexedDenial in denials)
            {
                int i = indexedDenial.Id;
                Denial denial = indexedDenial.Denial;
                string name = indexedDenial.Url;
                DenialItem denialItem = new DenialItem
                {
                    Id = i,
                    ServiceId = denial.ServiceId,
                    Name = name,
                    StartWorking = denial.StartWorking,
                    Time = denial.Time.ToString()
                };
                DenialsPanel.Add(denialItem);
            }
            thread = new Thread(Run);
            thread.Start();
        }

        private void Run()
        {
            while (true)
            {
                Thread.Sleep(1000);
                services = trackingService.Statuses;
                var denials = trackingService.GetDenials();
                SetStatuses();
                SetDenials(denials);
            }
        }

        public void AddService(string type, string url, string checkUrl, int timeCheck = 10)
        {
            int id = trackingService.AddService(type, url, checkUrl, timeCheck);
            ServiceItem item = new ServiceItem
            {
                Id = id,
                Type = type,
                Name = url,
                CheckUrl = checkUrl,
                TimeCheck = timeCheck,
                IsAlive = false
            };
            Panel.Add(item);
        }

        public void UpdateService(int Id, string type, string adress, int checkTime = 10)
        {
            trackingService.UpdateService(Id, type, adress, checkTime);
        }

        public void DeleteService(int id)
        {
            services.TryRemove(id, out bool _);
            trackingService.DeleteService(id);
            List<DenialItem> toDelete = new List<DenialItem>();
            foreach (var item in DenialsPanel)
            {
                if (item.ServiceId == id)
                {
                    toDelete.Add(item);
                }
            }
            foreach (var item in toDelete)
                DenialsPanel.Remove(item);
        }

        public void DeleteDenial(int id)
        {
            trackingService.DeleteDenial(id);
        }

        private void SetStatuses()
        {
            Window.Dispatcher.Invoke(() =>
            {
                foreach (ServiceItem serviceItem in Panel)
                {
                    if (services.ContainsKey(serviceItem.Id))
                    {
                        serviceItem.IsAlive = services[serviceItem.Id];
                    }
                }
            });
        }

        private void SetDenials(List<IndexedDenial> denials)
        {
            Window.Dispatcher.Invoke(() =>
            {
                foreach (var indexedDenial in denials)
                {
                    int i = indexedDenial.Id;
                    Denial denial = indexedDenial.Denial;
                    string name = indexedDenial.Url;
                    DenialItem denialItem = new DenialItem
                    {
                        Id = i,
                        ServiceId = denial.ServiceId,
                        Name = name,
                        StartWorking = denial.StartWorking,
                        Time = denial.Time.ToString()
                    };
                    DenialsPanel.Add(denialItem);
                }
            });
        }

        public void Close()
        {
            try
            {
                thread.Abort();
                trackingService.Save(loadFromDb);
                Properties.Settings.Default.loadFromDB = loadFromDb;
                Properties.Settings.Default.Save();
            }
            catch (Exception e)
            {
                Logger.Error("Error occured while closing app", e);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged([CallerMemberName] string prop = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
		}
	}
}
