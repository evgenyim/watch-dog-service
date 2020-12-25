using GUI.ViewModel;
using GUI.ViewModel.Commands;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;

namespace GUI.Model
{
    public class ServiceItem: INotifyPropertyChanged
    {
        private int id;
        private string name;
        private string type;
        private string checkUrl;
        private int timeCheck;
        private bool isAlive;
        private Brush fill;
        public static ApplicationViewModel parent;

        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        public string Type
        {
            get => type;
            set
            {
                type = value;
                OnPropertyChanged("Type");
            }
        }

        public string CheckUrl
        {
            get => checkUrl;
            set
            {
                checkUrl = value;
            }
        }

        public int TimeCheck
        {
            get => timeCheck;
            set
            {
                timeCheck = value;
            }
        }

        public bool IsAlive
        {
            get => isAlive;
            set
            {
                isAlive = value;
                if (isAlive)
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
        
        public int Id
        {
            get => id;
            set
            {
                id = value;
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
                           MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this service?", "Delete", MessageBoxButton.OKCancel);
                           switch (result)
                           {
                               case MessageBoxResult.OK:
                                   parent.Panel.Remove(this);
                                   parent.DeleteService(id);
                                   break;
                               case MessageBoxResult.Cancel:
                                   break;
                           }
                       }));
			}
		}

        private RelayCommand updateCommand;
        public RelayCommand UpdateCommand
        {
            get
            {
                return updateCommand ??
                       (updateCommand = new RelayCommand(obj =>
                       {
                            OnPropertyChanged("CheckUrl");
                            OnPropertyChanged("TimeCheck");
                            parent.UpdateService(id, type, checkUrl, timeCheck);
                           MaterialDesignThemes.Wpf.PopupBox.ClosePopupCommand.Execute(null, null);
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
