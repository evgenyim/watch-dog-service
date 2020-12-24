using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.ComponentModel;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Model.ServiceStorage;
using Controller.TrackingService;

namespace GUI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ConcurrentDictionary<int, bool> dict = new ConcurrentDictionary<int, bool>();

        private TrackingService trackingService = new TrackingService();
        private Thread thread;
        private bool loadFromDb;
        public MainWindow()
        {
            InitializeComponent();

            loadFromDb = Properties.Settings.Default.loadFromDB;

            if (loadFromDb)
            {
                loadDB.IsChecked = true;
                loadFile.IsChecked = false;
            }
            else {
                loadDB.IsChecked = false;
                loadFile.IsChecked = true;
            }

            List<Service> list;
            List<IndexedDenial> denials;
            list = trackingService.LoadServices(loadFromDb);
            denials = trackingService.LoadDenials(loadFromDb);
            foreach (var service_ in list)
            {
                if (service_ is WebService service)
                {
                    ServiceGrid s = new ServiceGrid(service.Id, "WebService", service.Url, service.CheckUrl, service.TimeCheck, false);
                    panel.Children.Add(s);
                }
            }
            foreach (var item in denials)
            {
                int i = item.Id;
                Denial d = item.Denial;
                string name = item.Url;
                DenialGrid dg = new DenialGrid(i, d.ServiceId, name, d.StartWorking, d.Time);
                denialsPanel.Children.Add(dg);
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
                Dispatcher.Invoke(() =>
                {
                    foreach (ServiceGrid g in panel.Children)
                    {
                        if (dict.ContainsKey(g.id))
                        {
                            g.setStatus(dict[g.id]);
                        }
                    }
                    foreach (var item in den)
                    {
                        int i = item.Id;
                        Denial d = item.Denial;
                        string name = item.Url;
                        DenialGrid dg = new DenialGrid(i, d.ServiceId, name, d.StartWorking, d.Time);
                        denialsPanel.Children.Add(dg);
                    }
                });
               
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddForm f = new AddForm();
            f.Owner = this;
            f.Show();
        }

        private void Button_Click_Check(object sender, RoutedEventArgs e)
        {
            Task.Run(() => {
                List<Status> l = trackingService.CheckServices();
                foreach (var s in l)
                {
                    dict[s.ServiceId] = s.IsAlive;
                }
                Dispatcher.BeginInvoke(DispatcherPriority.Render,
                         new Action(() => {
                             foreach (ServiceGrid g in panel.Children)
                             {
                                 if (dict.ContainsKey(g.id))
                                 {
                                     g.setStatus(dict[g.id]);
                                 }
                             }
                         }));
                MessageBox.Show("Services checked!");

            });
            
        }

        private void LoadDB_Checked(object sender, RoutedEventArgs e)
        {
            loadFromDb = true;
        }

        private void LoadFile_Checked(object sender, RoutedEventArgs e)
        {
            loadFromDb = false;
        }

        public void AddService(string type, string url, string adress, int checkTime=10)
        {
            int id = trackingService.AddService(type, url, adress, checkTime);
            ServiceGrid s = new ServiceGrid(id, type, url, adress, checkTime, false);
            panel.Children.Add(s);
        }

        public void UpdateService(int Id, string type, string adress, int checkTime = 10)
        {
            trackingService.UpdateService(Id, type, adress, checkTime);
            foreach(ServiceGrid child in panel.Children)
            {
                if (child.id == Id)
                {
                    child.Update(adress, checkTime);
                    break;
                }
            }
        }

        public void DeleteService(int id)
        {
            dict.TryRemove(id, out bool _);
            trackingService.DeleteService(id);
            List<DenialGrid> toDelete = new List<DenialGrid>();
            foreach (DenialGrid item in denialsPanel.Children)
            {
                if (item.serviceId == id)
                {
                    toDelete.Add(item);
                }
            }
            foreach (var item in toDelete)
                denialsPanel.Children.Remove(item);
        }

        public void DeleteDenial(int id)
        {
            trackingService.DeleteDenial(id);
        }

        void Window_Closing(object sender, CancelEventArgs e)
        {
            thread.Abort();
            trackingService.SaveServices(loadFromDb);
            trackingService.SaveDenials(loadFromDb);
            Properties.Settings.Default.loadFromDB = loadFromDb;
            Properties.Settings.Default.Save();
        }
    }
}
