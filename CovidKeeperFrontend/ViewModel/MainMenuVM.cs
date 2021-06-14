using CovidKeeperFrontend.Model;
using CovidKeeperFrontend.Model.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovidKeeperFrontend.ViewModel
{
    public class MainMenuVM : INotifyPropertyChanged
    {
        public MainMenuModel model;
        public MainMenuVM(MainMenuModel modelCreated)
        {
            this.model = modelCreated;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        public int VM_MinutesBreakForMailsProperty
        {
            set 
            { 
                model.MinutesBreakForMailsProperty = value;
            }
        }
        private string minutesBreakForMails;

        public string MinutesBreakForMailsProperty
        {
            get { return minutesBreakForMails; }
            set 
            { 
                minutesBreakForMails = value;
                NotifyPropertyChanged("MinutesBreakForMailsProperty");
            }
        }

        public NotifyTaskCompletion<int> RefreshDataAsync { get; private set; }
        public void RefreshData()
        {
            RefreshDataAsync = new NotifyTaskCompletion<int>(this.model.RefreshDataAsync());
        }

    }
}
