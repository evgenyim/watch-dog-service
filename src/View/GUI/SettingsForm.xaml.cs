using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GUI
{
    /// <summary>
    /// Логика взаимодействия для SettingsForm.xaml
    /// </summary>
    public partial class SettingsForm : Window
    {
        private string type;
        private int Id;
        public SettingsForm(int Id, string type, string url, string checkUrl, int checkTime)
        {
            InitializeComponent();
            textUrl.Text = url;
            textCheckUrl.Text = checkUrl;
            textCheckTime.Text = checkTime.ToString();
            this.type = type;
            this.Id = Id;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            string adress = textCheckUrl.Text;
            string checkTime = textCheckTime.Text;
            if (checkTime != "")
                ((MainWindow)Owner).UpdateService(Id, type, adress, Int32.Parse(checkTime));
            else
                ((MainWindow)Owner).UpdateService(Id, type, adress);
            Close();
        }
    }
}
