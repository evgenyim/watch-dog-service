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
                    ServiceGrid s = new ServiceGrid(item.Item1, "WebService", service.url, service.checkUrl, service.timeCheck, false);
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
        private Button deleteBtn;
        private Button settingsBtn;
        public string name;
        private string url;
        private string type;
        public int id;
        private string checkUrl;
        private int checkTime;
        public ServiceGrid(int id, string type, string url, string checkUrl, int checkTime, bool isAlive)
        {
            r = new Rectangle
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Width = 15
            };
            setStatus(isAlive);
            name_ = new TextBlock
            {
                Text = url,
                FontSize = 15,
                Margin = new Thickness(20, 0, 0, 0)
            };
            name = url;
            this.url = url;
            this.type = type;
            deleteBtn = new Button
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                Width = 50,
                Content = new MaterialDesignThemes.Wpf.PackIcon { Kind = MaterialDesignThemes.Wpf.PackIconKind.Delete },
                Margin = new Thickness(0, 0, 10, 0)
            };
            deleteBtn.Click += Delete_Button_Click;
            settingsBtn = new Button
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                Width = 50,
                Content = new MaterialDesignThemes.Wpf.PackIcon { Kind = MaterialDesignThemes.Wpf.PackIconKind.Cog },
                Margin = new Thickness(0, 0, 63, 0)
            };
            settingsBtn.Click += Settings_Button_Click;
            Children.Add(r);
            Children.Add(name_);
            Children.Add(deleteBtn);
            Children.Add(settingsBtn);
            this.id = id;
            this.checkUrl = checkUrl;
            this.checkTime = checkTime;
        }

        public void setStatus(bool status)
        {
            isAlive = status;
            if (isAlive)
            {
                r.Fill = new SolidColorBrush(Color.FromRgb(0, 113, 226));
            }
            else
            {
                r.Fill = new SolidColorBrush(Color.FromRgb(33, 33, 33));
            }
        }

        public void Update(string adress, int checkTime)
        {
            checkUrl = adress;
            this.checkTime = checkTime;
        }

        private void Delete_Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this service?", "Delete", MessageBoxButton.OKCancel);
            switch (result)
            {
                case MessageBoxResult.OK:
                    var parent = VisualTreeHelper.GetParent(this);
                    var parentAsPanel = parent as Panel;
                    parentAsPanel.Children.Remove(this);
                    MainWindow parentWindow = (MainWindow)Window.GetWindow(parent);
                    parentWindow.DeleteService(id);
                    break;
                case MessageBoxResult.Cancel:
                    break;
            }
        }

        private void Settings_Button_Click(object sender, RoutedEventArgs e)
        {
            SettingsForm f = new SettingsForm(id, type, url, checkUrl, checkTime);
            var parent = VisualTreeHelper.GetParent(this);
            MainWindow parentWindow = (MainWindow)Window.GetWindow(parent);
            f.Owner = parentWindow;
            f.Show();
        }
    }
}
