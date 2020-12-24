using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GUI
{
    public class DenialGrid : MaterialDesignThemes.Wpf.ColorZone
    {
        private Grid grid = new Grid();
        private TextBlock name;
        private TextBlock time;
        public int id;
        public int serviceId;
        private Button deleteBtn;


        public DenialGrid(int id, int serviceId, string name, bool status, DateTime time)
        {
            Mode = MaterialDesignThemes.Wpf.ColorZoneMode.Custom;
            if (status)
            {
                Background = new SolidColorBrush(Color.FromRgb(0, 113, 226));
                Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            }
            else
            {
                Background = new SolidColorBrush(Color.FromRgb(33, 33, 33));
                Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            }
            this.name = new TextBlock
            {
                Text = name,
                FontSize = 15,
                Margin = new Thickness(20, 0, 0, 0)
            };

            this.time = new TextBlock
            {
                Text = time.ToString(),
                FontSize = 15,
                Margin = new Thickness(300, 0, 0, 0)
            };
            this.id = id;
            this.serviceId = serviceId;
            deleteBtn = new Button
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                Width = 50,
                Content = new MaterialDesignThemes.Wpf.PackIcon { Kind = MaterialDesignThemes.Wpf.PackIconKind.Delete },
                Margin = new Thickness(0, 0, 10, 0)
            };
            deleteBtn.Click += Delete_Button_Click;
            AddChild(grid);
            grid.Children.Add(this.name);
            grid.Children.Add(this.time);
            grid.Children.Add(this.deleteBtn);
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
                    parentWindow.DeleteDenial(id);
                    break;
                case MessageBoxResult.Cancel:
                    break;
            }
        }
    }
}
