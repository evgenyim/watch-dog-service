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
        Dictionary<int, bool> dict = new Dictionary<int, bool>();

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
            List<Tuple<int, Status>> l = t.CheckServices();
            foreach(var s in l)
            {
                dict[s.Item1] = s.Item2.getStatus();
            }
            foreach (ServiceGrid g in panel.Children)
            {
                if (dict.ContainsKey(g.id))
                {
                    g.setStatus(dict[g.id]);
                }
            }
            MessageBox.Show("Services checked!");
        }


        public void AddService(string url, string adress, int checkTime)
        {
            int id = t.AddWebservice(url, adress, checkTime);
            ServiceGrid s = new ServiceGrid(id, url, false);
            panel.Children.Add(s);
        }

        public void DeleteService(int id)
        {
            dict.Remove(id);
            t.DeleteService(id);
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
        public ServiceGrid(int id, string url, bool isAlive)
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
            this.id = id;
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
