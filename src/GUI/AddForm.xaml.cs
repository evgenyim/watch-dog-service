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


namespace GUI
{
    /// <summary>
    /// Логика взаимодействия для AddForm.xaml
    /// </summary>
    public partial class AddForm : Window
    {
        public AddForm()
        {
            InitializeComponent();
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
                ((MainWindow)Owner).AddService(url, adress, Int32.Parse(checkTime));
                Close();
            } else
            {
                MessageBox.Show("Please, select service type");
            }
        }
    }
}
