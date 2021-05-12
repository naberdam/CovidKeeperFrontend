using CovidKeeperFrontend.HelperClasses;
using CovidKeeperFrontend.Model;
using LiveCharts;
using LiveCharts.Helpers;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static CovidKeeperFrontend.Model.WpfDatabase;
using static CovidKeeperFrontend.Views.StatisticalDataUserControl;

namespace CovidKeeperFrontend.ViewModel
{
    public class StatisticalDataVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public WpfDatabase model;

        public StatisticalDataVM(WpfDatabase modelCreated)
        {
            this.model = modelCreated;
            // Notify to view from model.
            model.PropertyChanged += delegate (Object sender, PropertyChangedEventArgs e)
            {
                NotifyPropertyChanged("VM_" + e.PropertyName);
            };
        }
        public void NotifyPropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }



        /*public string[] VM_StatisticsOptionListEnumProperty
        {
            get
            {
                WpfDatabase.StatisticsOptionListEnum[] statisticsOptionListEnum = this.model.StatisticsOptionListEnumProperty;
                if (statisticsOptionListEnum == null)
                {
                    return default;
                }
                List<string> statisticsOptionListConverted = new List<string>();
                foreach (var item in statisticsOptionListEnum)
                {
                    statisticsOptionListConverted.Add(item.ToString().Replace("_", " "));
                }
                if (statisticsOptionListConverted.Count == 0)
                {
                    return default;
                }
                return statisticsOptionListConverted.ToArray();
            }
        }*/
        /*public List<string> VM_StatisticsOptionListEnumProperty
        {
            get
            {
                WpfDatabase.StatisticsOptionListEnum[] statisticsOptionListEnum = this.model.StatisticsOptionListEnumProperty;
                if (statisticsOptionListEnum == null)
                {
                    return default;
                }
                List<string> statisticsOptionListConverted = new List<string>();
                foreach (var item in statisticsOptionListEnum)
                {
                    statisticsOptionListConverted.Add(item.ToString().Replace("_", " "));
                }
                if (statisticsOptionListConverted.Count == 0)
                {
                    return default;
                }
                return statisticsOptionListConverted;
            }
        }*/

        /*public string VM_SelectedValueOfStatisticsOptionListProperty
        {
            set
            {
                if (value == WpfDatabase.StatisticsOptionListEnum.Age_per_month.ToString().Replace("_", " "))
                {
                    this.model.SelectedValueOfStatisticsOptionListProperty = WpfDatabase.StatisticsOptionListEnum.Age_per_month;
                }
                else if (value == WpfDatabase.StatisticsOptionListEnum.Age_per_week.ToString().Replace("_", " "))
                {
                    this.model.SelectedValueOfStatisticsOptionListProperty = WpfDatabase.StatisticsOptionListEnum.Age_per_week;
                }
                else if (value == WpfDatabase.StatisticsOptionListEnum.Age_per_weekday.ToString().Replace("_", " "))
                {
                    this.model.SelectedValueOfStatisticsOptionListProperty = WpfDatabase.StatisticsOptionListEnum.Age_per_weekday;
                }
                else if (value == WpfDatabase.StatisticsOptionListEnum.Amount_events.ToString().Replace("_", " "))
                {
                    this.model.SelectedValueOfStatisticsOptionListProperty = WpfDatabase.StatisticsOptionListEnum.Amount_events;
                }
                else
                {
                    this.model.SelectedValueOfStatisticsOptionListProperty = WpfDatabase.StatisticsOptionListEnum.Nothing;
                }
            }
        }*/
        public StatisticsOptionListEnum VM_SelectedValueOfStatisticsOptionListProperty
        {
            set
            {
                this.model.SelectedValueOfStatisticsOptionListProperty = value;
            }
        }

        public Visibility VM_VisibilityOfLineGraphProperty
        {
            get { return this.model.VisibilityOfLineGraphProperty; }
        }
        public Visibility VM_VisibilityOfPieGraphProperty
        {
            get { return this.model.VisibilityOfPieGraphProperty; }
        }
        public Visibility VM_VisibilityOfAmountEventPerWeekTableProperty
        {
            get { return this.model.VisibilityOfAmountEventPerWeekTableProperty; }
        }
        public Visibility VM_VisibilityOfAmountEventPerMonthTableProperty
        {
            get { return this.model.VisibilityOfAmountEventPerMonthTableProperty; }
        }
        public Visibility VM_VisibilityOfAmountEventByWorkerTableProperty
        {
            get { return this.model.VisibilityOfAmountEventByWorkerTableProperty; }
        }
        public DataTable VM_AmountEventsByWorkerTableProperty
        {
            get { return model.AmountEventsByWorkerTableProperty; }
        }
        public List<GraphContent> VM_ColumnGraphProperty
        {
            get { return this.model.ColumnGraphProperty; }
        }
        public DateTime VM_StartDateInDatePickerProperty
        {
            get { return this.model.StartDateInDatePickerProperty.Date; }
        }
        public DateTime VM_SelectedDateStartInDatePickerProperty
        {
            set { this.model.SelectedDateStartInDatePickerProperty = value; }
        }
        public DateTime VM_SelectedDateEndInDatePickerProperty
        {
            set { this.model.SelectedDateEndInDatePickerProperty = value; }
        }
        public string VM_ColumnChartTitleProperty
        {
            get { return this.model.ColumnChartTitleProperty.ToString().Replace("_", " "); }
        }
        public string VM_ColumnChartSubTitleProperty
        {
            get { return this.model.ColumnChartSubTitleProperty.ToString().Replace("_", " "); }
        }
        public Visibility VM_VisibilityStartDatePickerProperty
        {
            get { return this.model.VisibilityStartDatePickerProperty; }
        }
        public Visibility VM_VisibilityEndDatePickerProperty
        {
            get { return this.model.VisibilityEndDatePickerProperty; }
        }
        public Visibility VM_VisibilityShowGraphInThisRangeProperty
        {
            get { return this.model.VisibilityShowGraphInThisRangeProperty; }
        }
        public string VM_IdWorkerForLineGraphProperty 
        {
            set { this.model.IdWorkerForLineGraphProperty = value; }
        }
    }
}
