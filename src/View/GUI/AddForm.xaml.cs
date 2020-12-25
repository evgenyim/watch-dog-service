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
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using GUI.ViewModel;

namespace GUI
{
    /// <summary>
    /// Логика взаимодействия для AddForm.xaml
    /// </summary>
    public partial class AddForm : Window
    {
        ApplicationViewModel parent;
        public AddForm(ApplicationViewModel avm)
        {
            InitializeComponent();
            parent = avm;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TextBlock selectedItem = (TextBlock)serviceList.SelectedItem;
            if (selectedItem != null)
            {
                string serviceType = selectedItem.Text;
                string url = textBoxUrl.Text;
                string adress = textBoxAdress.Text;
                string checkTime = textBoxCheckTime.Text;
                if (checkTime != "")
                    parent.AddService(serviceType, url, adress, Int32.Parse(checkTime));
                else
                    parent.AddService(serviceType, url, adress);
                Close();
            }
            else
            {
                MessageBox.Show("Please, select service type");
            }
        }
    }
}
