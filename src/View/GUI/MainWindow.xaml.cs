using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.ComponentModel;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Model.ServiceStorage;
using Controller.TrackingService;
using GUI.ViewModel;
using System.Windows.Input;
using System.Text.RegularExpressions;

namespace GUI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new ApplicationViewModel(this);
            
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        void Window_Closing(object sender, CancelEventArgs e)
        {
            ((ApplicationViewModel)DataContext).Close();
        }
    }
}
