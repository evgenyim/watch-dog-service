using GUI.ViewModel;
using GUI.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace GUI.Model
{
    public class DenialItem : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public string Time { get; set; }
        public int Id { get; set; }
        public int ServiceId { get; set; }
        private Brush fill;
        private bool startWorking;

        public static ApplicationViewModel parent;


        public bool StartWorking
        {
            get => startWorking;
            set
            {
                startWorking = value;
                if (startWorking)
                {
                    Fill = new SolidColorBrush(Color.FromRgb(0, 113, 226));
                }
                else
                {
                    Fill = new SolidColorBrush(Color.FromRgb(33, 33, 33));
                }
                OnPropertyChanged("IsAlive");
            }
        }
        public Brush Fill
        {
            get => fill;
            set
            {
                fill = value;
                OnPropertyChanged("Fill");
            }
        }

        #region Commands

        // команда добавления нового объекта
        private RelayCommand deleteCommand;
        public RelayCommand DeleteCommand
        {
            get
            {
                return deleteCommand ??
                       (deleteCommand = new RelayCommand(obj =>
                       {
                           MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this denial?", "Delete", MessageBoxButton.OKCancel);
                           switch (result)
                           {
                               case MessageBoxResult.OK:
                                   parent.DenialsPanel.Remove(this);
                                   parent.DeleteDenial(Id);
                                   break;
                               case MessageBoxResult.Cancel:
                                   break;
                           }
                       }));
            }
        }

        #endregion


        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
