using CovidKeeperFrontend.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovidKeeperFrontend.ViewModel
{
    public class HomeVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public WpfDatabase model;
        public HomeVM(WpfDatabase modelCreated) 
        {
            this.model = modelCreated;
            // Notify to view from model.
            model.PropertyChanged += delegate (Object sender, PropertyChangedEventArgs e)
            {
                NotifyPropertyChanged("VM_" + e.PropertyName);
            };
        }
        public void ActiveButtonClicked()
        {
            this.model.StartOrCloseProgram();
        }

        public string VM_HowManyWorkersWithoutMaskProperty
        {
            get { return model.HowManyWorkersWithoutMaskProperty; }
        }
        public string VM_ActiveButtonContentProperty
        {
            get 
            {
                if (this.model.ActiveButtonContentProperty == WpfDatabase.HandleFlag.Start)
                {
                    BackgroundColorActiveButtonProperty = System.Windows.Media.Brushes.Green;
                }
                else
                {
                    BackgroundColorActiveButtonProperty = System.Windows.Media.Brushes.Red;
                }
                return this.model.ActiveButtonContentProperty.ToString();
            }
        }
        public string VM_PercentageWorkersWithoutMaskTodayPerYesterdayProperty
        {
            get 
            {
                if (model.PercentageWorkersWithoutMaskTodayPerYesterdayProperty > 0)
                {
                    ForegroundColorPercentageWorkersProperty = System.Windows.Media.Brushes.Red;
                    return GetFormatPercentageWorkers(model.PercentageWorkersWithoutMaskTodayPerYesterdayProperty, "+");
                }
                else if (model.PercentageWorkersWithoutMaskTodayPerYesterdayProperty < 0)
                {
                    ForegroundColorPercentageWorkersProperty = System.Windows.Media.Brushes.Green;
                    return GetFormatPercentageWorkers(model.PercentageWorkersWithoutMaskTodayPerYesterdayProperty, "-");
                }
                else
                {
                    ForegroundColorPercentageWorkersProperty = System.Windows.Media.Brushes.Orange;
                    return String.Format("{0:0}%", model.PercentageWorkersWithoutMaskTodayPerYesterdayProperty);
                }
                
            }
        }
        private System.Windows.Media.Brush foregroundColorPercentageWorkers = System.Windows.Media.Brushes.DarkSeaGreen;

        public System.Windows.Media.Brush ForegroundColorPercentageWorkersProperty
        {
            get { return foregroundColorPercentageWorkers; }
            set
            {
                foregroundColorPercentageWorkers = value;
                NotifyPropertyChanged("ForegroundColorPercentageWorkersProperty");
            }
        }
        private string GetFormatPercentageWorkers(float percentageWorkers, string signPercentage)
        {
            if (percentageWorkers > 0 && percentageWorkers < 10)
            {
                return String.Format(signPercentage + "{0:0.0}%", percentageWorkers);
            }
            if (percentageWorkers >= 10 && percentageWorkers < 100)
            {
                return String.Format(signPercentage + "{0:00.0}%", percentageWorkers);
            }
            if (percentageWorkers >= 100 && percentageWorkers < 1000)
            {
                return String.Format(signPercentage + "{0:000}%", percentageWorkers);
            }
            if (percentageWorkers >= 1000 && percentageWorkers < 10000)
            {
                return String.Format(signPercentage + "{0:0000}%", percentageWorkers);
            }
            return String.Format(signPercentage + "{0:00000}%", percentageWorkers);
        }
        private System.Windows.Media.Brush backgroundColorActiveButton = System.Windows.Media.Brushes.DarkSeaGreen;

        public System.Windows.Media.Brush BackgroundColorActiveButtonProperty
        {
            get { return backgroundColorActiveButton; }
            set
            {
                backgroundColorActiveButton = value;
                NotifyPropertyChanged("BackgroundColorActiveButtonProperty");
            }
        }

        public void NotifyPropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
