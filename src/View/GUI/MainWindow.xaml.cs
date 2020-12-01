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
            if (loadFromDb)
                list = t.LoadServicesDB();
            else
                list = t.LoadServicesFile();
            foreach (var item in list)
            {
                if (item.Item2 is WebService service)
                {
                    ServiceGrid s = new ServiceGrid(item.Item1, service.url, service.checkUrl, service.timeCheck, false);
                    panel.Children.Add(s);
                }
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
                Dispatcher.Invoke(() =>
                {
                    foreach (ServiceGrid g in panel.Children)
                    {
                        if (dict.ContainsKey(g.id))
                        {
                            g.setStatus(dict[g.id]);
                        }
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
            ServiceGrid s = new ServiceGrid(id, url, adress, checkTime, false);
            panel.Children.Add(s);
        }

        public void DeleteService(int id)
        {
            dict.Remove(id);
            t.DeleteService(id);
        }

        void Window_Closing(object sender, CancelEventArgs e)
        {
            thread.Abort();
            if (loadFromDb)
                t.SaveServicesDB();
            else
                t.SaveServicesFile();

            Properties.Settings.Default.loadFromDB = loadFromDb;
            Properties.Settings.Default.Save();
        }
    }

    public class ServiceGrid : Grid
    {
        private Rectangle r;
        private TextBlock name_;
        private bool isAlive;
        private Button btn;
        public string name;
        public int id;
        private string checkUrl;
        private int timeCheck;
        public ServiceGrid(int id, string url, string checkUrl, int timeCheck, bool isAlive)
        {
            r = new Rectangle
            {
                HorizontalAlignment = HorizontalAlignment.Left
            };
            setStatus(isAlive);
            r.Width = 10;
            name_ = new TextBlock
            {
                Text = url,
                FontSize = 15,
                Margin = new Thickness(10, 0, 0, 0)
            };
            name = url;
            btn = new Button
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                Width = 80,
                Content = "Delete",
                FontSize = 10
            };
            btn.Click += Button_Click;
            Children.Add(r);
            Children.Add(name_);
            Children.Add(btn);
            this.id = id;
            this.checkUrl = checkUrl;
            this.timeCheck = timeCheck;
        }

        public void setStatus(bool status)
        {
            isAlive = status;
            if (isAlive)
            {
                r.Fill = new SolidColorBrush(Colors.Green);
            }
            else
            {
                r.Fill = new SolidColorBrush(Colors.Red);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var parent = VisualTreeHelper.GetParent(this);
            var parentAsPanel = parent as Panel;
            parentAsPanel.Children.Remove(this);
            MainWindow parentWindow = (MainWindow) Window.GetWindow(parent);
            parentWindow.DeleteService(id);

        }
    }
}
