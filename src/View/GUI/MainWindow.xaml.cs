using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using Controller;
using Model;
using Model.Other;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GUI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<int, bool> dict = new Dictionary<int, bool>();

        private TrackingService t = new TrackingService();
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

            List<Tuple<int, Service>> list;
            List<Tuple<int, Denial, string>> denials;
            if (loadFromDb)
            {
                list = t.LoadServicesDB();
                denials = t.LoadDenialsDB();
            }
            else
            {
                list = t.LoadServicesFile();
                denials = t.LoadDenialsFile();
            }
            foreach (var item in list)
            {
                if (item.Item2 is WebService service)
                {
                    ServiceGrid s = new ServiceGrid(item.Item1, "WebService", service.url, service.checkUrl, service.timeCheck, false);
                    panel.Children.Add(s);
                }
            }
            foreach (var item in denials)
            {
                int i = item.Item1;
                Denial d = item.Item2;
                string name = item.Item3;
                DenialGrid dg = new DenialGrid(i, d.serviceId, name, d.startWorking, d.time);
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
                dict = t.statuses;
                var den = t.GetDenials();
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
                        int i = item.Item1;
                        Denial d = item.Item2;
                        string name = item.Item3;
                        DenialGrid dg = new DenialGrid(i, d.serviceId, name, d.startWorking, d.time);
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
                List<Tuple<int, Status>> l = t.CheckServices();
                foreach (var s in l)
                {
                    dict[s.Item1] = s.Item2.getStatus();
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
            int id = t.AddService(type, url, adress, checkTime);
            ServiceGrid s = new ServiceGrid(id, type, url, adress, checkTime, false);
            panel.Children.Add(s);
        }

        public void UpdateService(int Id, string type, string adress, int checkTime = 10)
        {
            t.UpdateService(Id, type, adress, checkTime);
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
            dict.Remove(id);
            t.DeleteService(id);
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
            t.DeleteDenial(id);
        }

        void Window_Closing(object sender, CancelEventArgs e)
        {
            thread.Abort();
            if (loadFromDb)
            {
                t.SaveServicesDB();
                t.SaveDenialsDB();
            }
            else
            {
                t.SaveServicesFile();
                t.SaveDenialsFile();
            }

            Properties.Settings.Default.loadFromDB = loadFromDb;
            Properties.Settings.Default.Save();
        }
    }
}
