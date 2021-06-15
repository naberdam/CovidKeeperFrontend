﻿using CovidKeeperFrontend.HelperClasses;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CovidKeeperFrontend.Views.StatisticalDataUserControl;

namespace CovidKeeperFrontend.Model
{
    //This class is the model of StatisticalDataUserControl
    public class StatisticalDataModel : AbstractModel
    {
        //A dictionary that contains the work week as the keys and for each work week it contains the amount events as the value
        private readonly Dictionary<string, double> workWeekAndAmountEventsDict = new Dictionary<string, double>();
        //A dictionary that contains the month as the keys and for each month it contains the amount events as the value
        private readonly Dictionary<string, double> monthAndAmountEventsDict = new Dictionary<string, double>();

        //Property that defines the start date in StartDatePicker in StatisticalDataUserControl
        private DateTime startDateInDatePicker = default;
        public DateTime StartDateInDatePickerProperty
        {
            get { return startDateInDatePicker; }
            set
            {
                if (!startDateInDatePicker.Date.Equals(value.Date))
                {
                    startDateInDatePicker = value;
                    //Update the start date in the EndDatePicker to be from the beginning of StartDatePicker choose
                    StartDateInDatePickerAfterPickProperty = value;
                    NotifyPropertyChanged("StartDateInDatePickerProperty");
                }
            }
        }

        //Property that defines the selected date in StartDatePicker in StatisticalDataUserControl
        private DateTime selectedDateStartInDatePicker = default;
        public DateTime SelectedDateStartInDatePickerProperty
        {
            get { return selectedDateStartInDatePicker; }
            set
            {
                if (!selectedDateStartInDatePicker.Date.Equals(value.Date))
                {
                    selectedDateStartInDatePicker = value;
                    StartDateInDatePickerAfterPickProperty = value;
                }
            }
        }

        //Property that represent the selection of StartDatePicker and updates the start date of EndDatePicker
        private DateTime startDateInDatePickerAfterPick = default;
        public DateTime StartDateInDatePickerAfterPickProperty
        {
            get { return startDateInDatePickerAfterPick; }
            set
            {
                if (!startDateInDatePickerAfterPick.Date.Equals(value.Date))
                {
                    startDateInDatePickerAfterPick = value;
                    NotifyPropertyChanged("StartDateInDatePickerAfterPickProperty");
                }
            }
        }

        //Property that represent the id worker that the client wants to see a graph of amount events about him
        private string idWorkerForLineGraph = default;
        public string IdWorkerForLineGraphProperty
        {
            get { return idWorkerForLineGraph; }
            set
            {
                if (idWorkerForLineGraph != value && value != default)
                {
                    idWorkerForLineGraph = value;
                    GetAvgEventsPerWeekPerWorker(idWorkerForLineGraph);
                }
            }
        }

        //Property that helps the client to know what the dates that he will choose represent
        private string weekOrMonthForDateRangeText;
        public string WeekOrMonthForDateRangeTextProperty
        {
            get { return weekOrMonthForDateRangeText; }
            set
            {
                if (weekOrMonthForDateRangeText != value)
                {
                    weekOrMonthForDateRangeText = value;
                    NotifyPropertyChanged("WeekOrMonthForDateRangeTextProperty");
                }
            }
        }

        //Property that represent datatable of the workers with the amount events
        private DataTable amountEventsByWorkerTable = default;
        public DataTable AmountEventsByWorkerTableProperty
        {
            get { return amountEventsByWorkerTable; }
            set
            {
                if (amountEventsByWorkerTable == default || !AreTablesTheSame(amountEventsByWorkerTable, value))
                {
                    amountEventsByWorkerTable = value;
                    NotifyPropertyChanged("AmountEventsByWorkerTableProperty");
                }
            }
        }

        //Enum that represent the title of the month's or week's column graph
        public enum ColumnChartTitleEnum
        {
            Average_Per_Week = 1,
            Average_Per_Month = 2
        }

        //Property that represent the column chart title
        private ColumnChartTitleEnum columnChartTitle;
        public ColumnChartTitleEnum ColumnChartTitleProperty
        {
            get { return columnChartTitle; }
            set
            {
                if (columnChartTitle != value)
                {
                    columnChartTitle = value;
                    NotifyPropertyChanged("ColumnChartTitleProperty");
                }
            }
        }

        //Enum for sub title for the column graph
        public enum ColumnChartSubTitleEnum
        {
            Each_column_represent_one_week_in_a_specific_year = 1,
            Each_column_represent_one_month_in_a_specific_year = 2
        }

        //Property that represent the sub title for the column graph
        private ColumnChartSubTitleEnum columnChartSubTitle;
        public ColumnChartSubTitleEnum ColumnChartSubTitleProperty
        {
            get { return columnChartSubTitle; }
            set
            {
                if (columnChartSubTitle != value)
                {
                    columnChartSubTitle = value;
                    NotifyPropertyChanged("ColumnChartSubTitleProperty");
                }
            }
        }

        //Property that represent the which graph the client wants to see now
        private StatisticsOptionListEnum selectedValueOfStatisticsOptionList = StatisticsOptionListEnum.Nothing;
        public StatisticsOptionListEnum SelectedValueOfStatisticsOptionListProperty
        {
            get { return selectedValueOfStatisticsOptionList; }
            set
            {
                if (selectedValueOfStatisticsOptionList != value)
                {
                    selectedValueOfStatisticsOptionList = value;
                    switch (value)
                    {
                        //If the client wants the graph per week
                        case StatisticsOptionListEnum.AGE_PER_WEEK:
                            GetAvgEventsPerWeek();
                            ColumnChartTitleProperty = ColumnChartTitleEnum.Average_Per_Week;
                            ColumnChartSubTitleProperty = ColumnChartSubTitleEnum.Each_column_represent_one_week_in_a_specific_year;
                            break;
                        //If the client wants the graph per month
                        case StatisticsOptionListEnum.AGE_PER_MONTH:
                            GetAvgEventsPerMonth();
                            ColumnChartTitleProperty = ColumnChartTitleEnum.Average_Per_Month;
                            ColumnChartSubTitleProperty = ColumnChartSubTitleEnum.Each_column_represent_one_month_in_a_specific_year;
                            break;
                        //If the client wants the graph per weekday
                        case StatisticsOptionListEnum.AGE_PER_WEEKDAY:
                            GetAvgEventsPerWeekday();
                            break;
                        //If the client wants the datatable that represent the amount events for each worker
                        case StatisticsOptionListEnum.TOTAL_EVENTS:
                            GetAmountEventsByWorker();
                            break;
                        default:
                            break;
                    }
                }

            }
        }

        //Enum for representing the month as string instead of numbers
        public enum MonthEnumForGraphs
        {
            Jan = 1,
            Feb = 2,
            Mar = 3,
            Apr = 4,
            May = 5,
            Jun = 6,
            Jul = 7,
            Aug = 8,
            Sep = 9,
            Oct = 10,
            Nov = 11,
            Dec = 12
        }

        //Enum for representing the weekday as string instead of numbers
        public enum DayEnumForGraphs
        {
            Sunday = 1,
            Monday = 2,
            Tuesday = 3,
            Wednesday = 4,
            Thursday = 5,
            Friday = 6,
            Saturday = 7
        }

        //Property that represent the values of the column graph
        private List<GraphContent> columnGraph;
        public List<GraphContent> ColumnGraphProperty
        {
            get { return columnGraph; }
            set
            {
                if (columnGraph == null || columnGraph.SequenceEqual(value) ||
                    columnGraph.Except(value).ToList().Count() > 0 || value.Except(columnGraph).ToList().Count() > 0)
                {
                    columnGraph = value;
                    NotifyPropertyChanged("ColumnGraphProperty");
                }
            }
        }

        //Enum for representing if the graph is for week or month
        public enum WeekOrMonth
        {
            Week = 1,
            Month = 2,
            Nothing = 0
        }

        public StatisticalDataModel()
        {
            RefreshData = new NotifyTaskCompletion<int>(RefreshDataAsync());
        }

        //Refresh data of StatisticalDataUserControl
        public NotifyTaskCompletion<int> RefreshData { get; private set; }
        public override async Task<int> RefreshDataAsync()
        {
            await GetMinDateInHistoryEvents();
            return default;
        }

        //Function that check if the given datatables are the same
        public static bool AreTablesTheSame(DataTable tbl1, DataTable tbl2)
        {
            //If the amount of the rows or columns in each datatable are equal
            if (tbl1.Rows.Count != tbl2.Rows.Count || tbl1.Columns.Count != tbl2.Columns.Count)
                return false;
            //make loop over the datatable to check if they are equal
            for (int i = 0; i < tbl1.Rows.Count; i++)
            {
                for (int c = 0; c < tbl1.Columns.Count; c++)
                {
                    if (!Equals(tbl1.Rows[i][c], tbl2.Rows[i][c]))
                        return false;
                }
            }
            return true;
        }

        //Function that initializes the dictionary of work week as keys and amount events as values
        private void InitialWorkWeekAndAmountEventsDict()
        {
            //Gets the Calendar instance associated with a CultureInfo.
            CultureInfo myCI = new CultureInfo("en-US");
            Calendar myCal = myCI.Calendar;

            //Gets the DTFI properties required by GetWeekOfYear.
            CalendarWeekRule myCWR = myCI.DateTimeFormat.CalendarWeekRule;
            DayOfWeek myFirstDOW = myCI.DateTimeFormat.FirstDayOfWeek;
            int firstWorkWeekWithEvents = myCal.GetWeekOfYear(StartDateInDatePickerProperty, myCWR, myFirstDOW);
            int lastWorkWeek = myCal.GetWeekOfYear(DateTime.Now, myCWR, myFirstDOW);
            //Iterate over the years while starting to iterate from the year of start date of StartDatePicker
            for (int i = StartDateInDatePickerProperty.Year; i <= DateTime.Now.Year; i++)
            {
                DateTime LastDay = new System.DateTime(i, 12, 31);
                int weekAmount = myCal.GetWeekOfYear(LastDay, myCWR, myFirstDOW);
                int firstWorkWeek = 1;
                //If we are in the first iteration, so define firstWorkWeek to be the work week of the start date
                if (i == StartDateInDatePickerProperty.Year)
                {
                    firstWorkWeek = firstWorkWeekWithEvents;
                }
                //If i is this year, so define weekAmount to be the work week now
                if (i == DateTime.Now.Year)
                {
                    weekAmount = lastWorkWeek;
                }
                //Iterate over the work weeks
                for (; firstWorkWeek <= weekAmount; firstWorkWeek++)
                {
                    workWeekAndAmountEventsDict.Add(firstWorkWeek + "-" + i, 0);
                }
            }
        }

        //Function that initializes the dictionary of month as keys and amount events as values
        private void InitialMonthAndAmountEventsDict()
        {
            // Uses the default calendar of the InvariantCulture.
            Calendar myCal = CultureInfo.InvariantCulture.Calendar;
            int firstMonthWithEvents = myCal.GetMonth(StartDateInDatePickerProperty);
            int lastMonth = myCal.GetMonth(DateTime.Now);
            //Iterate over the years while starting to iterate from the year of start date of StartDatePicker
            for (int i = StartDateInDatePickerProperty.Year; i <= DateTime.Now.Year; i++)
            {
                int monthAmount = myCal.GetMonthsInYear(i);
                int firstMonth = 1;
                //If we are in the first iteration, so define firstMonth to be the month of the start date
                if (i == StartDateInDatePickerProperty.Year)
                {
                    firstMonth = firstMonthWithEvents;
                }
                //If i is this year, so define monthAmount to be the month now
                if (i == DateTime.Now.Year)
                {
                    monthAmount = lastMonth;
                }
                //Iterate over the months
                for (; firstMonth <= monthAmount; firstMonth++)
                {
                    monthAndAmountEventsDict.Add(((MonthEnumForGraphs)firstMonth).ToString() + "-" + i, 0);
                }
            }
        }

        //Function that gets the minimum date in History_events table in SQL and updates the StartDateInDatePickerProperty
        private async Task GetMinDateInHistoryEvents()
        {
            await Task.Run(() =>
            {
                if (StartDateInDatePickerProperty.Equals(default))
                {
                    string minDateQuery = "select min(Time_of_event) from [dbo].[History_Events]";
                    object[] minDate = QuerySelectOfOneRow(minDateQuery);
                    //If we have events, so we want to define the StartDateInDatePickerProperty to be the minimum date
                    if (minDate != null)
                    {
                        StartDateInDatePickerProperty = Convert.ToDateTime(minDate[0]);
                    }
                    //We do not have events, so we want to define the StartDateInDatePickerProperty to be the date of today
                    else
                    {
                        StartDateInDatePickerProperty = DateTime.Now;
                    }
                    InitialWorkWeekAndAmountEventsDict();
                    InitialMonthAndAmountEventsDict();
                }
            });
        }

        //Function that using the given dictionary that contains the values to the graph and returns a converted list of GraphContent
        private List<GraphContent> ConvertDictToListGraphContent(Dictionary<string, double> dictToListGraphContent)
        {
            List<GraphContent> avgEvents = new List<GraphContent>();
            foreach (var item in dictToListGraphContent)
            {
                avgEvents.Add(new GraphContent() { WorkWeek = item.Key, AvgValue = item.Value });
            }
            return avgEvents;
        }

        //Function that gets the average of the events per week and updates the ColumnGraphProperty for representing the graph to the client
        public void GetAvgEventsPerWeek()
        {
            string avgEventsPerWeekQuery = "Select DISTINCT DATEPART(WEEK, Time_of_event) AS WW, DATEPART(year, Time_of_event) AS Year, " +
                "COUNT(CONVERT(date, Time_of_event)) / 5.0 AS Avg " +
                "from[dbo].[History_Events] " +
                "group by DATEPART(WEEK, Time_of_event), DATEPART(year, Time_of_event); ";
            List<object[]> avgEventsPerWeekList = QuerySelectOfMultiRows(avgEventsPerWeekQuery);

            if (avgEventsPerWeekList != null)
            {
                //Set the values in workWeekAndAmountEventsDict to zero for initializing
                SetValuesInDictToZero(this.workWeekAndAmountEventsDict);
                foreach (var item in avgEventsPerWeekList)
                {
                    string weekStr = item[0].ToString() + "-" + item[1].ToString();
                    this.workWeekAndAmountEventsDict[weekStr] = Convert.ToDouble(item[2]);
                }
                ColumnGraphProperty = ConvertDictToListGraphContent(this.workWeekAndAmountEventsDict);
            }
        }


        public void GetAvgEventsPerWeekWithRange(DateTime startDate, DateTime endDate)
        {
            string avgEventsPerWeekQuery = "Select DISTINCT DATEPART(WEEK, Time_of_event) AS WW, DATEPART(year, Time_of_event) AS Year, " +
                "COUNT(CONVERT(date, Time_of_event)) / 5.0 AS Avg " +
                "from[dbo].[History_Events] " +
                "group by DATEPART(WEEK, Time_of_event), DATEPART(year, Time_of_event); ";
            List<object[]> avgEventsPerWeekList = QuerySelectOfMultiRows(avgEventsPerWeekQuery);

            if (avgEventsPerWeekList != null)
            {
                CultureInfo myCI = new CultureInfo("en-US");
                Calendar myCal = myCI.Calendar;
                int firstWorkWeekWithEvents = myCal.GetWeekOfYear(startDate, myCI.DateTimeFormat.CalendarWeekRule, myCI.DateTimeFormat.FirstDayOfWeek);
                int lastWorkWeek = myCal.GetWeekOfYear(endDate, myCI.DateTimeFormat.CalendarWeekRule, myCI.DateTimeFormat.FirstDayOfWeek);
                Dictionary<string, double> workWeekAndAmountEventsDictForRange = GetWorkWeekAndAmountEventsDictForRange(firstWorkWeekWithEvents,
                    lastWorkWeek, myCI, startDate.Year, endDate.Year);
                if (startDate.DayOfWeek != DayOfWeek.Sunday)
                {
                    int daysOffset = DayOfWeek.Saturday - startDate.DayOfWeek;
                    startDate = startDate.AddDays((6 - daysOffset) * -1);
                }
                if (endDate.DayOfWeek != DayOfWeek.Saturday)
                {
                    int daysOffset = DayOfWeek.Saturday - endDate.DayOfWeek;
                    endDate = endDate.AddDays(daysOffset);
                }
                foreach (var item in avgEventsPerWeekList)
                {
                    DateTime temp = new DateTime(Convert.ToInt32(item[1]), 1, 1);
                    temp = temp.AddDays(Convert.ToInt32(item[0]) * 7 + 1);
                    if (startDate.Ticks > temp.Ticks || endDate.Ticks < temp.Ticks)
                    {
                        continue;
                    }
                    string weekStr = item[0].ToString() + "-" + item[1].ToString();
                    workWeekAndAmountEventsDictForRange[weekStr] = Convert.ToDouble(item[2]);
                }
                WeekOrMonthForDateRangeTextProperty = "weeks";
                ColumnGraphProperty = ConvertDictToListGraphContent(workWeekAndAmountEventsDictForRange);
            }
        }

        public void GetAvgEventsPerMonth()
        {
            string avgEventsPerMonthQuery = "Select DISTINCT DATEPART(month, Time_of_event) AS Month, DATEPART(year, Time_of_event) AS Year, " +
                "COUNT(CONVERT(date, Time_of_event)) / 20.0 AS Avg " +
                "from[dbo].[History_Events] " +
                "group by DATEPART(month, Time_of_event), DATEPART(year, Time_of_event); ";
            List<object[]> avgEventsPerMonthList = QuerySelectOfMultiRows(avgEventsPerMonthQuery);

            if (avgEventsPerMonthList != null)
            {
                SetValuesInDictToZero(this.monthAndAmountEventsDict);
                foreach (var item in avgEventsPerMonthList)
                {
                    MonthEnumForGraphs monthEnum = (MonthEnumForGraphs)Enum.Parse(typeof(MonthEnumForGraphs), item[0].ToString());
                    string weekStr = monthEnum.ToString() + "-" + item[1].ToString();
                    this.monthAndAmountEventsDict[weekStr] = Convert.ToDouble(item[2]);
                }
                ColumnGraphProperty = ConvertDictToListGraphContent(this.monthAndAmountEventsDict); ;
            }
        }


        public void GetAvgEventsPerMonthWithRange(DateTime startDate, DateTime endDate)
        {
            string avgEventsPerMonthQuery = "Select DISTINCT DATEPART(month, Time_of_event) AS Month, DATEPART(year, Time_of_event) AS Year, " +
                "COUNT(CONVERT(date, Time_of_event)) / 20.0 AS Avg " +
                "from[dbo].[History_Events] " +
                "group by DATEPART(month, Time_of_event), DATEPART(year, Time_of_event); ";
            List<object[]> avgEventsPerMonthList = QuerySelectOfMultiRows(avgEventsPerMonthQuery);

            if (avgEventsPerMonthList != null)
            {
                int minMonth = startDate.Month;
                int maxMonth = endDate.Month;
                int minYear = startDate.Year;
                int maxYear = endDate.Year;
                DateTime minDate = new DateTime(minYear, minMonth, 1);
                DateTime maxDate = new DateTime(maxYear, maxMonth, DateTime.DaysInMonth(maxYear, maxMonth));
                Dictionary<string, double> monthAndAmountEventsDictForRange = GetMonthAndAmountEventsDictForRange(minMonth, maxMonth, minYear, maxYear);
                foreach (var item in avgEventsPerMonthList)
                {
                    DateTime temp = new DateTime(Convert.ToInt32(item[1]), Convert.ToInt32(item[0]), 15);
                    if (minDate.Ticks > temp.Ticks || maxDate.Ticks < temp.Ticks)
                    {
                        continue;
                    }
                    MonthEnumForGraphs monthEnum = (MonthEnumForGraphs)Enum.Parse(typeof(MonthEnumForGraphs), item[0].ToString());
                    string monthStr = monthEnum.ToString() + "-" + item[1].ToString();
                    monthAndAmountEventsDictForRange[monthStr] = Convert.ToDouble(item[2]);
                }
                WeekOrMonthForDateRangeTextProperty = "months";
                ColumnGraphProperty = ConvertDictToListGraphContent(monthAndAmountEventsDictForRange);
            }
        }
        private Dictionary<string, double> GetMonthAndAmountEventsDictForRange(int minMonth, int maxMonth, int minYear, int maxYear)
        {
            Dictionary<string, double> monthAndAmountEventsDictForRange = new Dictionary<string, double>();
            Calendar myCal = CultureInfo.InvariantCulture.Calendar;
            for (int i = minYear; i <= maxYear; i++)
            {
                int monthAmount = myCal.GetMonthsInYear(i);
                int firstMonth = 1;
                if (i == minYear)
                {
                    firstMonth = minMonth;
                }
                if (i == maxYear)
                {
                    monthAmount = maxMonth;
                }
                for (; firstMonth <= monthAmount; firstMonth++)
                {
                    monthAndAmountEventsDictForRange.Add(((MonthEnumForGraphs)firstMonth).ToString() + "-" + i, 0);
                }
            }
            return monthAndAmountEventsDictForRange;
        }
        private Dictionary<string, double> GetWorkWeekAndAmountEventsDictForRange(int firstWorkWeekWithEvents, int lastWorkWeek, CultureInfo myCI, int minYear, int maxYear)
        {
            Dictionary<string, double> workWeekAndAmountEventsDictForRange = new Dictionary<string, double>();
            Calendar myCal = myCI.Calendar;
            for (int i = minYear; i <= maxYear; i++)
            {
                DateTime LastDay = new System.DateTime(i, 12, 31);
                int weekAmount = myCal.GetWeekOfYear(LastDay, myCI.DateTimeFormat.CalendarWeekRule, myCI.DateTimeFormat.FirstDayOfWeek);
                int firstWorkWeek = 1;
                if (i == minYear)
                {
                    firstWorkWeek = firstWorkWeekWithEvents;
                }
                if (i == maxYear)
                {
                    weekAmount = lastWorkWeek;
                }
                for (; firstWorkWeek <= weekAmount; firstWorkWeek++)
                {
                    workWeekAndAmountEventsDictForRange.Add(firstWorkWeek + "-" + i, 0);
                }
            }
            return workWeekAndAmountEventsDictForRange;
        }

        public void GetAvgEventsPerWeekday()
        {
            string avgEventsPerWeekdayQuery = "Select DISTINCT DATEPART(weekday, Time_of_event) AS Weekday, " +
                "COUNT(CONVERT(date, Time_of_event)) / 5.0 AS Avg " +
                "from[dbo].[History_Events] " +
                "group by DATEPART(weekday, Time_of_event); ";
            List<object[]> avgEventsPerWeekdayList = QuerySelectOfMultiRows(avgEventsPerWeekdayQuery);

            if (avgEventsPerWeekdayList != null)
            {
                List<GraphContent> avgEventsPerWeekday = new List<GraphContent>();
                foreach (var item in avgEventsPerWeekdayList)
                {
                    DayEnumForGraphs dayEnum = (DayEnumForGraphs)Enum.Parse(typeof(DayEnumForGraphs), item[0].ToString());
                    avgEventsPerWeekday.Add(new GraphContent() { WorkWeek = dayEnum.ToString(), AvgValue = Convert.ToDouble(item[1]) });
                }
                ColumnGraphProperty = avgEventsPerWeekday;
            }
        }
        public void GetAvgEventsPerWeekdayWithRange(DateTime startDate, DateTime endDate)
        {
            string avgEventsPerWeekdayQuery = "Select DISTINCT DATEPART(weekday, Time_of_event) AS Weekday, " +
                "COUNT(CONVERT(date, Time_of_event)) / 5.0 AS Avg " +
                "from [dbo].[History_Events] " +
                "where (Time_of_event > '" + startDate.ToString("MM/dd/yyyy") + "' and Time_of_event < '" + endDate.ToString("MM/dd/yyyy") + "') " +
                "group by DATEPART(weekday, Time_of_event); ";
            List<object[]> avgEventsPerWeekdayList = QuerySelectOfMultiRows(avgEventsPerWeekdayQuery);

            if (avgEventsPerWeekdayList != null)
            {
                List<GraphContent> avgEventsPerWeekday = new List<GraphContent>();
                foreach (var item in avgEventsPerWeekdayList)
                {
                    DayEnumForGraphs dayEnum = (DayEnumForGraphs)Enum.Parse(typeof(DayEnumForGraphs), item[0].ToString());
                    avgEventsPerWeekday.Add(new GraphContent() { WorkWeek = dayEnum.ToString(), AvgValue = Convert.ToDouble(item[1]) });
                }
                WeekOrMonthForDateRangeTextProperty = "weeks";
                ColumnGraphProperty = avgEventsPerWeekday;
            }
        }
        public void GetAmountEventsByWorker()
        {
            string amountEventsPerWorkerQuery = "Select DISTINCT Id_worker, FullName, " +
                "COUNT(Id_worker) AS Count " +
                "from[dbo].[History_Events] " +
                "INNER JOIN [dbo].[Workers] ON [dbo].[History_Events].Id_worker=[dbo].[Workers].Id " +
                "group by Id_worker, FullName " +
                "order by count desc; ";
            /*SqlCommand command = new SqlCommand();
            command.CommandText = amountEventsPerWorkerQuery;
            command.Connection = sqlConnection;
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
            DataTable dataTableAmountEventsPerWorker = new DataTable("History_Events");
            sqlDataAdapter.Fill(dataTableAmountEventsPerWorker);*/
            DataTable dataTableAmountEventsPerWorker = GetDataTableByQuery(amountEventsPerWorkerQuery, "History_Events");
            dataTableAmountEventsPerWorker.Columns.Add("LineSeriesGraph", typeof(SeriesCollection));
            dataTableAmountEventsPerWorker.Columns.Add("LabelsGraph", typeof(string[]));
            dataTableAmountEventsPerWorker.Columns.Add("TitleGraph", typeof(string));
            foreach (DataRow row in dataTableAmountEventsPerWorker.Rows)
            {
                row["LineSeriesGraph"] = default;
                row["LabelsGraph"] = default;
                row["TitleGraph"] = default;
            }
            AmountEventsByWorkerTableProperty = dataTableAmountEventsPerWorker;
        }

        public void GetAmountEventsByWorkerWithRange(DateTime startDate, DateTime endDate)
        {
            string amountEventsPerWorkerQuery = "Select DISTINCT Id_worker, FullName, " +
                "COUNT(Id_worker) AS Count " +
                "from[dbo].[History_Events] " +
                "INNER JOIN [dbo].[Workers] ON [dbo].[History_Events].Id_worker=[dbo].[Workers].Id " +
                "where (Time_of_event > '" + startDate.ToString("MM/dd/yyyy") + "' and Time_of_event < '" + endDate.ToString("MM/dd/yyyy") + "') " +
                "group by Id_worker, FullName " +
                "order by count desc; ";
            DataTable dataTableAmountEventsPerWorker = GetDataTableByQuery(amountEventsPerWorkerQuery, "History_Events");
            dataTableAmountEventsPerWorker.Columns.Add("LineSeriesGraph", typeof(SeriesCollection));
            dataTableAmountEventsPerWorker.Columns.Add("LabelsGraph", typeof(string[]));
            dataTableAmountEventsPerWorker.Columns.Add("TitleGraph", typeof(string));
            foreach (DataRow row in dataTableAmountEventsPerWorker.Rows)
            {
                row["LineSeriesGraph"] = default;
                row["LabelsGraph"] = default;
                row["TitleGraph"] = default;
            }
            WeekOrMonthForDateRangeTextProperty = "days";
            AmountEventsByWorkerTableProperty = dataTableAmountEventsPerWorker;
        }

        public void GetAvgEventsPerWeekPerWorker(string idWorker)
        {
            string avgEventsPerWeekPerWorkerQuery = "Select DISTINCT DATEPART(WEEK, Time_of_event) AS WW, DATEPART(year, Time_of_event) AS Year, " +
                "COUNT(CONVERT(date, Time_of_event)) AS Avg " +
                "from[dbo].[History_Events] where Id_worker = " + idWorker +
                " group by DATEPART(WEEK, Time_of_event), DATEPART(year, Time_of_event); ";
            List<object[]> avgEventsPerWeekPerWorkerList = QuerySelectOfMultiRows(avgEventsPerWeekPerWorkerQuery);

            if (avgEventsPerWeekPerWorkerList != null)
            {
                SetValuesInDictToZero(this.workWeekAndAmountEventsDict);
                foreach (var item in avgEventsPerWeekPerWorkerList)
                {
                    string weekStr = item[0].ToString() + "-" + item[1].ToString();
                    this.workWeekAndAmountEventsDict[weekStr] = Convert.ToDouble(item[2]);
                }
                DataTable tempAmountEventsByWorkerTableProperty = AmountEventsByWorkerTableProperty;
                DataRow[] foundRows = tempAmountEventsByWorkerTableProperty.Select("Id_worker = " + idWorker);
                foundRows[0]["LineSeriesGraph"] = GetLineSeriesGraph("Total Events Per Week Of " + foundRows[0]["FullName"].ToString());
                foundRows[0]["LabelsGraph"] = (new List<string>(this.workWeekAndAmountEventsDict.Keys)).ToArray();
                foundRows[0]["TitleGraph"] = WeekOrMonth.Week;
                AmountEventsByWorkerTableProperty = tempAmountEventsByWorkerTableProperty;
            }
        }
        private SeriesCollection GetLineSeriesGraph(string titleForLegend)
        {
            ChartValues<double> values = new ChartValues<double>();
            foreach (var item in this.workWeekAndAmountEventsDict.Values)
            {
                values.Add(item);
            }
            return new SeriesCollection
                    {
                        new LineSeries
                        {
                            Values =  values,
                            Title = titleForLegend
                        }
                    };
        }
        private Dictionary<string, double> SetValuesInDictToZero(Dictionary<string, double> dictToSet)
        {
            foreach (var key in dictToSet.Keys.ToList())
            {
                dictToSet[key] = 0;
            }
            return dictToSet;
        }
    }
}
