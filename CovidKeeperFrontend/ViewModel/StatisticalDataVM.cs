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
using static CovidKeeperFrontend.Views.StatisticalDataUserControl;

namespace CovidKeeperFrontend.ViewModel
{
    public class StatisticalDataVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public StatisticalDataModel model;

        public StatisticalDataVM(StatisticalDataModel modelCreated)
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

        //Property of binding SelectedValueOfStatisticsOptionListProperty to view
        public StatisticsOptionListEnum VM_SelectedValueOfStatisticsOptionListProperty
        {
            set { this.model.SelectedValueOfStatisticsOptionListProperty = value; }
        }
        public bool GraphInRangeOrNotProperty
        {
            set { this.model.GraphInRangeOrNotProperty = value; }
        }
        //Property of binding AmountEventsByWorkerTableProperty to view
        public DataTable VM_AmountEventsByWorkerTableProperty
        {
            get { return model.AmountEventsByWorkerTableProperty; }
        }
        //Property of binding ColumnGraphProperty to view
        public List<GraphContent> VM_ColumnGraphProperty
        {
            get { return this.model.ColumnGraphProperty; }
        }
        //Property of binding StartDateInDatePickerProperty to view
        public DateTime VM_StartDateInDatePickerProperty
        {
            get { return this.model.StartDateInDatePickerProperty.Date; }
        }
        //Property of binding SelectedDateStartInDatePickerProperty to view
        public DateTime VM_SelectedDateStartInDatePickerProperty
        {
            set { this.model.SelectedDateStartInDatePickerProperty = value; }
        }
        //Property of binding SelectedDateEndInDatePickerProperty to view
        public DateTime VM_SelectedDateEndInDatePickerProperty
        {
            set { this.model.SelectedDateEndInDatePickerProperty = value; }
        }
        //Property of binding StartDateInDatePickerAfterPickProperty to view
        public DateTime VM_StartDateInDatePickerAfterPickProperty
        {
            get { return this.model.StartDateInDatePickerAfterPickProperty; }
        }
        //Property of binding EndDateForTextRepresentationProperty to view
        public string VM_EndDateForTextRepresentationProperty
        {
            get { return this.model.EndDateForTextRepresentationProperty.ToString("dd/MM/yy"); }
        }
        //Property of binding StartDateForTextRepresentationProperty to view
        public string VM_StartDateForTextRepresentationProperty
        {
            get { return this.model.StartDateForTextRepresentationProperty.ToString("dd/MM/yy"); }
        }
        //Property of binding ColumnChartTitleProperty to view and delete '_'
        public string VM_ColumnChartTitleProperty
        {
            get { return this.model.ColumnChartTitleProperty.ToString().Replace("_", " "); }
        }
        //Property of binding ColumnChartBelowTitleProperty to view and delete '_'
        public string VM_ColumnChartBelowTitleProperty
        {
            get { return this.model.ColumnChartBelowTitleProperty; }
        }
        //Property of binding ColumnChartSubTitleProperty to view and delete '_'
        public string VM_ColumnChartSubTitleProperty
        {
            get { return this.model.ColumnChartSubTitleProperty.ToString().Replace("_", " "); }
        }
        //Property of binding WeekOrMonthForDateRangeTextProperty to view
        public string VM_WeekOrMonthForDateRangeTextProperty
        {
            get { return this.model.WeekOrMonthForDateRangeTextProperty; }
        }
        //Property of binding IdWorkerForLineGraphProperty to view
        public string VM_IdWorkerForLineGraphProperty 
        {
            set { this.model.IdWorkerForLineGraphProperty = value; }
        }
        //Function that calls to model function for getting the average events per month with date range
        public void GetAvgEventsPerMonthWithRange(DateTime startDate, DateTime endDate)
        {
            this.model.GetAvgEventsPerMonthWithRange(startDate, endDate);
        }
        //Function that calls to model function for getting the average events per week with date range
        public void GetAvgEventsPerWeekWithRange(DateTime startDate, DateTime endDate)
        {
            this.model.GetAvgEventsPerWeekWithRange(startDate, endDate);
        }
        //Function that calls to model function for getting the average events per weekday with date range
        public void GetAvgEventsPerWeekdayWithRange(DateTime startDate, DateTime endDate)
        {
            this.model.GetAvgEventsPerWeekdayWithRange(startDate, endDate);
        }
        //Function that calls to model function for getting the amount events for each worker with date range
        public void GetAmountEventsByWorkerWithRange(DateTime startDate, DateTime endDate)
        {
            this.model.GetAmountEventsByWorkerWithRange(startDate, endDate);
        }
        public NotifyTaskCompletion<int> RefreshDataAsync { get; private set; }
        public void RefreshData()
        {
            RefreshDataAsync = new NotifyTaskCompletion<int>(this.model.RefreshDataAsync());
        }
    }
}
