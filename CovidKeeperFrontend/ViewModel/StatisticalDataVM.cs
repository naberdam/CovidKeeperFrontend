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

        /*public Visibility VM_VisibilityOfLineGraphProperty
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
        }*/
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
        /*public Visibility VM_VisibilityStartDatePickerProperty
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
        }*/
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
