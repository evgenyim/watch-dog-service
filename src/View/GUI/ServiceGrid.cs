using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GUI
{
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
