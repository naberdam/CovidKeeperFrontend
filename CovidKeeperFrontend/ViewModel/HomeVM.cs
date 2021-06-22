using CovidKeeperFrontend.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CovidKeeperFrontend.ViewModel
{
    public class HomeVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public HomeModel model;
        public HomeVM(HomeModel modelCreated)
        {
            this.model = modelCreated;
            // Notify to view from model.
            model.PropertyChanged += delegate (Object sender, PropertyChangedEventArgs e)
            {
                NotifyPropertyChanged("VM_" + e.PropertyName);
            };
        }

        //Function that reponsible for what happen when the client click on the active button
        public async Task ActiveButtonClicked()
        {
            await this .model.StartOrCloseProgram();
        }

        //Property of binding HowManyWorkersWithoutMaskTodayProperty to view
        public string VM_HowManyWorkersWithoutMaskTodayProperty
        {
            get { return model.HowManyWorkersWithoutMaskTodayProperty; }
        }
        //Property of binding HowManyEventsTodayProperty to view
        public string VM_HowManyEventsTodayProperty
        {
            get { return model.HowManyEventsTodayProperty; }
        }
        //Property of binding PercentageWorkersWithoutMaskTodayPerYesterdayProperty to view
        public string VM_PercentageWorkersWithoutMaskTodayPerYesterdayProperty
        {
            get 
            {
                if (model.PercentageWorkersWithoutMaskTodayPerYesterdayProperty > 0)
                {
                    //Set the ForegroundColorPercentageWorkersProperty to be red
                    ForegroundColorPercentageWorkersProperty = System.Windows.Media.Brushes.Red;
                    return GetFormatPercentageWorkers(model.PercentageWorkersWithoutMaskTodayPerYesterdayProperty, "+");
                }
                else if (model.PercentageWorkersWithoutMaskTodayPerYesterdayProperty < 0)
                {
                    //Set the ForegroundColorPercentageWorkersProperty to be green
                    ForegroundColorPercentageWorkersProperty = System.Windows.Media.Brushes.Green;
                    return GetFormatPercentageWorkers(model.PercentageWorkersWithoutMaskTodayPerYesterdayProperty * (-1), "-");
                }
                else
                {
                    //Set the ForegroundColorPercentageWorkersProperty to be orange
                    ForegroundColorPercentageWorkersProperty = System.Windows.Media.Brushes.Orange;
                    return String.Format("{0:0}%", model.PercentageWorkersWithoutMaskTodayPerYesterdayProperty);
                }
                
            }
        }
        //Property of binding ForegroundColorPercentageWorkersProperty to view
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
        //Property that defines the format of percentage workers
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
        //Property of binding ActiveButtonContentProperty to view
        public string VM_ActiveButtonContentProperty
        {
            get
            {
                if (this.model.ActiveButtonContentProperty == HomeModel.HandleFlag.Start)
                {
                    //Set the ActiveButton to green
                    BackgroundColorActiveButtonProperty = System.Windows.Media.Color.FromRgb(0, 93, 21);
                    BackgroundColorActiveButtonSecondProperty = System.Windows.Media.Color.FromRgb( 0, 170, 39);
                    BackgroundColorActiveButtonThirdProperty = System.Windows.Media.Color.FromRgb(0, 245, 11);
                    
                }
                else
                {
                    //Set the ActiveButton to red
                    BackgroundColorActiveButtonProperty = System.Windows.Media.Color.FromRgb(245, 0, 0);
                    BackgroundColorActiveButtonSecondProperty = System.Windows.Media.Color.FromRgb(170, 0, 0);
                    BackgroundColorActiveButtonThirdProperty = System.Windows.Media.Color.FromRgb(93, 0, 0);
                }
                return this.model.ActiveButtonContentProperty.ToString();
            }
        }
        //Property that defines the background the first color of th ActiveButton
        private System.Windows.Media.Color backgroundColorActiveButton = System.Windows.Media.Color.FromRgb(0, 93, 21);
        public System.Windows.Media.Color BackgroundColorActiveButtonProperty
        {
            get { return backgroundColorActiveButton; }
            set
            {
                backgroundColorActiveButton = value;
                NotifyPropertyChanged("BackgroundColorActiveButtonProperty");
            }
        }
        //Property that defines the background the second color of th ActiveButton
        private System.Windows.Media.Color backgroundColorActiveButtonSecond = System.Windows.Media.Color.FromRgb(0, 170, 39);
        public System.Windows.Media.Color BackgroundColorActiveButtonSecondProperty
        {
            get { return backgroundColorActiveButtonSecond; }
            set
            {
                backgroundColorActiveButtonSecond = value;
                NotifyPropertyChanged("BackgroundColorActiveButtonSecondProperty");
            }
        }
        //Property that defines the background the third color of th ActiveButton
        private System.Windows.Media.Color backgroundColorActiveButtonThird = System.Windows.Media.Color.FromRgb(0, 245, 11);
        public System.Windows.Media.Color BackgroundColorActiveButtonThirdProperty
        {
            get { return backgroundColorActiveButtonThird; }
            set
            {
                backgroundColorActiveButtonThird = value;
                NotifyPropertyChanged("BackgroundColorActiveButtonThirdProperty");
            }
        }
        public void NotifyPropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
        public NotifyTaskCompletion<int> RefreshDataAsync { get; private set; }
        public void RefreshData()
        {
            RefreshDataAsync = new NotifyTaskCompletion<int>(this.model.RefreshDataAsync());
        }
    }
}
