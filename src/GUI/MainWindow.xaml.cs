using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Controller;
using Model;

namespace GUI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Dictionary<string, bool> dict = new Dictionary<string, bool>();

        TrackingService t = new TrackingService();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddForm f = new AddForm();
            f.Owner = this;
            f.Show();
        }

        private void Button_Click_Check(object sender, RoutedEventArgs e)
        {
            List<Status> l = t.CheckServices();
            foreach(Status s in l)
            {
                dict[s.getUrl()] = s.getStatus();
            }
            foreach (ServiceGrid g in panel.Children)
            {
                if (dict.ContainsKey(g.name))
                {
                    g.setStatus(dict[g.name]);
                }
            }
            MessageBox.Show("Services checked!");
        }


        public void AddService(string url, string adress, int checkTime)
        {
            t.AddWebservice(url, adress, checkTime);
            ServiceGrid s = new ServiceGrid(url, false);
            panel.Children.Add(s);
        }
    }

    public class ServiceGrid : Grid
    {
        Rectangle r;
        TextBlock name_;
        bool isAlive;
        Button btn;
        public string name;
        public ServiceGrid(string url, bool isAlive)
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
                Width = 30
            };
            btn.Click += Button_Click;
            Children.Add(r);
            Children.Add(name_);
            Children.Add(btn);
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
        }
    }
}
