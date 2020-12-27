using Controller.TrackingService;
using GUI.Model;
using GUI.ViewModel.Commands;
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
		private ConcurrentDictionary<int, bool> dict = new ConcurrentDictionary<int, bool>();

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
                                   dict[s.ServiceId] = s.IsAlive;
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
            this.Window = window;
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
            foreach (var item in denials)
            {
                int i = item.Id;
                Denial d = item.Denial;
                string name = item.Url;
                DenialItem di = new DenialItem
                {
                    Id = i,
                    ServiceId = d.ServiceId,
                    Name = name,
                    StartWorking = d.StartWorking,
                    Time = d.Time.ToString()
                };
                DenialsPanel.Add(di);
            }
            thread = new Thread(Run);
            thread.Start();
        }

        private void Run()
        {
            while (true)
            {
                Thread.Sleep(1000);
                dict = trackingService.Statuses;
                var den = trackingService.GetDenials();
                SetStatuses();
                SetDenials(den);
            }
        }

        public void AddService(string type, string url, string checkUrl, int timeCheck = 10)
        {
            int id = trackingService.AddService(type, url, checkUrl, timeCheck);
            ServiceItem s = new ServiceItem
            {
                Id = id,
                Type = type,
                Name = url,
                CheckUrl = checkUrl,
                TimeCheck = timeCheck,
                IsAlive = false
            };
            Panel.Add(s);
        }

        public void UpdateService(int Id, string type, string adress, int checkTime = 10)
        {
            trackingService.UpdateService(Id, type, adress, checkTime);
        }

        public void DeleteService(int id)
        {
            dict.TryRemove(id, out bool _);
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
                foreach (ServiceItem g in Panel)
                {
                    if (dict.ContainsKey(g.Id))
                    {
                        g.IsAlive = dict[g.Id];
                    }
                }
            });
        }

        private void SetDenials(List<IndexedDenial> denials)
        {
            Window.Dispatcher.Invoke(() =>
            {
                foreach (var item in denials)
                {
                    int i = item.Id;
                    Denial d = item.Denial;
                    string name = item.Url;
                    DenialItem di = new DenialItem
                    {
                        Id = i,
                        ServiceId = d.ServiceId,
                        Name = name,
                        StartWorking = d.StartWorking,
                        Time = d.Time.ToString()
                    };
                    DenialsPanel.Add(di);
                }
            });
        }

        public void Close()
        {
            thread.Abort();
            trackingService.Save(loadFromDb);
            Properties.Settings.Default.loadFromDB = loadFromDb;
            Properties.Settings.Default.Save();
        }

        public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged([CallerMemberName] string prop = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
		}
	}
}
