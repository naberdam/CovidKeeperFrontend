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


        public StatisticsOptionListEnum VM_SelectedValueOfStatisticsOptionListProperty
        {
            set
            {
                this.model.SelectedValueOfStatisticsOptionListProperty = value;
            }
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
        public DateTime VM_StartDateInDatePickerAfterPickProperty
        {
            get { return this.model.StartDateInDatePickerAfterPickProperty; }
        }
        public string VM_ColumnChartTitleProperty
        {
            get { return this.model.ColumnChartTitleProperty.ToString().Replace("_", " "); }
        }
        public string VM_ColumnChartSubTitleProperty
        {
            get { return this.model.ColumnChartSubTitleProperty.ToString().Replace("_", " "); }
        }
        public string VM_WeekOrMonthForDateRangeTextProperty
        {
            get { return this.model.WeekOrMonthForDateRangeTextProperty; }
        }
        public string VM_IdWorkerForLineGraphProperty 
        {
            set { this.model.IdWorkerForLineGraphProperty = value; }
        }
        public void GetAvgEventsPerMonthWithRange(DateTime startDate, DateTime endDate)
        {
            this.model.GetAvgEventsPerMonthWithRange(startDate, endDate);
        }
        public void GetAvgEventsPerWeekWithRange(DateTime startDate, DateTime endDate)
        {
            this.model.GetAvgEventsPerWeekWithRange(startDate, endDate);
        }
        public void GetAvgEventsPerWeekdayWithRange(DateTime startDate, DateTime endDate)
        {
            this.model.GetAvgEventsPerWeekdayWithRange(startDate, endDate);
        }
        public void GetAmountEventsByWorkerWithRange(DateTime startDate, DateTime endDate)
        {
            this.model.GetAmountEventsByWorkerWithRange(startDate, endDate);
        }
    }
}
