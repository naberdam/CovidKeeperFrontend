using CovidKeeperFrontend.HelperClasses;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using static CovidKeeperFrontend.Views.StatisticalDataUserControl;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;

namespace CovidKeeperFrontend.Model
{
    public class WpfDatabase : INotifyPropertyChanged
    {
        private string connectionString;
        private SqlConnection sqlConnection;
        private Dictionary<string, double> workWeekAndAmountEventsDict = new Dictionary<string, double>();
        private Dictionary<string, double> monthAndAmountEventsDict = new Dictionary<string, double>();
        private CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;" +
                "AccountName=faceimages2;" +
                "AccountKey=vlaKfbwxn8eU1kZGo3KjuFIsgQ0BGot1MRCvs6x0mB923Yx2FOXv4XQ82Hgi/l4iKb4iM/DSNcAeezmYYxxFxw==;" +
                "EndpointSuffix=core.windows.net");

        public event PropertyChangedEventHandler PropertyChanged;

        private string howManyWorkersWithoutMask = "";
        public string HowManyWorkersWithoutMaskProperty
        {
            get
            {
                return howManyWorkersWithoutMask;
            }
            set
            {
                if (howManyWorkersWithoutMask != value)
                {
                    howManyWorkersWithoutMask = value;
                    NotifyPropertyChanged("HowManyWorkersWithoutMaskProperty");
                }
            }
        }

        private DataTable workerDetailsTable = default;
        public DataTable WorkerDetailsTableProperty 
        {
            get { return workerDetailsTable; } 
            set
            {
                if (workerDetailsTable != value)
                {
                    workerDetailsTable = value;
                    NotifyPropertyChanged("WorkerDetailsTableProperty");
                }
            }
        }
        private DataTable searchWorkerDetailsTable = default;
        public DataTable SearchWorkerDetailsTableProperty
        {
            get { return searchWorkerDetailsTable; }
            set
            {
                if (searchWorkerDetailsTable != value)
                {
                    searchWorkerDetailsTable = value;
                    NotifyPropertyChanged("SearchWorkerDetailsTableProperty");
                }
            }
        }
        private float percentageWorkersWithoutMaskTodayPerYesterday = 0;

        public float PercentageWorkersWithoutMaskTodayPerYesterdayProperty
        {
            get { return percentageWorkersWithoutMaskTodayPerYesterday; }
            set 
            { 
               if (percentageWorkersWithoutMaskTodayPerYesterday != value)
                {
                    percentageWorkersWithoutMaskTodayPerYesterday = value;
                    NotifyPropertyChanged("PercentageWorkersWithoutMaskTodayPerYesterdayProperty");
                }
            }
        }
        public enum StartOrClose
        {
            Close = 0,
            Start = 1
        }

        private StartOrClose activeButtonContent;

        public StartOrClose ActiveButtonContentProperty
        {
            get { return activeButtonContent; }
            set
            {
                if (activeButtonContent != value)
                {
                    activeButtonContent = value;
                    NotifyPropertyChanged("ActiveButtonContentProperty");
                }
            }
        }


        private int indexOfMenuList = -1;

        public int IndexOfMenuListProperty
        {
            set 
            {
                // The first time the client entered the program
                if (indexOfMenuList == -1)
                {
                    RefreshDataHome();
                    RefreshDataWorkersTable();
                    indexOfMenuList = value;
                    return;
                }
                if (indexOfMenuList != value)
                {
                    switch (value)
                    {
                        case 0:
                            RefreshDataHome();
                            break;
                        case 1:
                            break;
                        case 2:
                            RefreshSearchWorkers();
                            break;
                        case 3:
                            RefreshStatisticalData();
                            break;
                        default:
                            break;
                    }
                    indexOfMenuList = value;
                }                 
            }
        }

        private DataTable amountEventsByWorkerTable = default;

        public DataTable AmountEventsByWorkerTableProperty
        {
            get { return amountEventsByWorkerTable; }
            set 
            {
                if (amountEventsByWorkerTable == default || AreTablesTheSame(amountEventsByWorkerTable, value))
                {
                    amountEventsByWorkerTable = value;
                    NotifyPropertyChanged("AmountEventsByWorkerTableProperty");
                }
            }
        }

        public enum ColumnChartTitleEnum
        {
            Average_Per_Week = 1,
            Average_Per_Month = 2
        }

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
        public enum ColumnChartSubTitleEnum
        {
            Each_column_represent_one_week_in_a_specific_year = 1,
            Each_column_represent_one_month_in_a_specific_year = 2
        }
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
                        case StatisticsOptionListEnum.AGE_PER_WEEK:
                            GetAvgEventsPerWeek();
                            ColumnChartTitleProperty = ColumnChartTitleEnum.Average_Per_Week;
                            ColumnChartSubTitleProperty = ColumnChartSubTitleEnum.Each_column_represent_one_week_in_a_specific_year;
                            break;
                        case StatisticsOptionListEnum.AGE_PER_MONTH:
                            GetAvgEventsPerMonth();
                            ColumnChartTitleProperty = ColumnChartTitleEnum.Average_Per_Month;
                            ColumnChartSubTitleProperty = ColumnChartSubTitleEnum.Each_column_represent_one_month_in_a_specific_year;
                            break;
                        case StatisticsOptionListEnum.AGE_PER_WEEKDAY:
                            GetAvgEventsPerWeekday();
                            break;
                        case StatisticsOptionListEnum.TOTAL_EVENTS:
                            GetAmountEventsByWorker();
                            break;
                        default:
                            break;
                    }
                }
                
            }
        }
        private Visibility visibilityOfLineGraph = Visibility.Hidden;

        public Visibility VisibilityOfLineGraphProperty
        {
            get { return visibilityOfLineGraph; }
            set 
            {
                if (visibilityOfLineGraph != value)
                {
                    visibilityOfLineGraph = value;
                    NotifyPropertyChanged("VisibilityOfLineGraphProperty");
                }                
            }
        }

        private Visibility visibilityOfPieGraph = Visibility.Hidden;

        public Visibility VisibilityOfPieGraphProperty
        {
            get { return visibilityOfPieGraph; }
            set
            {
                if (visibilityOfPieGraph != value)
                {
                    visibilityOfPieGraph = value;
                    NotifyPropertyChanged("VisibilityOfPieGraphProperty");
                }
            }
        }

        private Visibility visibilityOfAmountEventByWorkerTable = Visibility.Hidden;

        public Visibility VisibilityOfAmountEventByWorkerTableProperty
        {
            get { return visibilityOfAmountEventByWorkerTable; }
            set
            {
                if (visibilityOfAmountEventByWorkerTable != value)
                {
                    visibilityOfAmountEventByWorkerTable = value;
                    NotifyPropertyChanged("VisibilityOfAmountEventByWorkerTableProperty");
                }
            }
        }

        private Visibility visibilityOfAmountEventPerWeekTable = Visibility.Hidden;

        public Visibility VisibilityOfAmountEventPerWeekTableProperty
        {
            get { return visibilityOfAmountEventPerWeekTable; }
            set
            {
                if (visibilityOfAmountEventPerWeekTable != value)
                {
                    visibilityOfAmountEventPerWeekTable = value;
                    NotifyPropertyChanged("VisibilityOfAmountEventPerWeekTableProperty");
                }
            }
        }
        private Visibility visibilityOfAmountEventPerMonthTable = Visibility.Hidden;

        public Visibility VisibilityOfAmountEventPerMonthTableProperty
        {
            get { return visibilityOfAmountEventPerMonthTable; }
            set
            {
                if (visibilityOfAmountEventPerMonthTable != value)
                {
                    visibilityOfAmountEventPerMonthTable = value;
                    NotifyPropertyChanged("VisibilityOfAmountEventPerMonthTableProperty");
                }
            }
        }
        private Visibility visibilityStartDatePicker = Visibility.Hidden;

        public Visibility VisibilityStartDatePickerProperty
        {
            get { return visibilityStartDatePicker; }
            set
            {
                if (visibilityStartDatePicker != value)
                {
                    visibilityStartDatePicker = value;
                    NotifyPropertyChanged("VisibilityStartDatePickerProperty");
                }
            }
        }
        private Visibility visibilityEndDatePicker = Visibility.Hidden;

        public Visibility VisibilityEndDatePickerProperty
        {
            get { return visibilityEndDatePicker; }
            set
            {
                if (visibilityEndDatePicker != value)
                {
                    visibilityEndDatePicker = value;
                    NotifyPropertyChanged("VisibilityEndDatePickerProperty");
                }
            }
        }
        private Visibility visibilityShowGraphInThisRange = Visibility.Hidden;

        public Visibility VisibilityShowGraphInThisRangeProperty
        {
            get { return visibilityShowGraphInThisRange; }
            set
            {
                if (visibilityShowGraphInThisRange != value)
                {
                    visibilityShowGraphInThisRange = value;
                    NotifyPropertyChanged("VisibilityShowGraphInThisRangeProperty");
                }
            }
        }

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

        public enum DayEnumForGraphs
        {
            Sunday = 1,
            Monday = 2,
            Tuesday = 3,
            Wednesday = 4,
            Thursday = 5,
            Friday = 6,
            Saturday= 7
        }
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

        public enum WeekOrMonth 
        {
            Week = 1,
            Month = 2,
            Nothing = 0
        }


        private WeekOrMonth titleOfLineChartOfWorker = WeekOrMonth.Nothing;

        public WeekOrMonth TitleOfLineChartOfWorkerProperty
        {
            get { return titleOfLineChartOfWorker; }
            set 
            {
                if (titleOfLineChartOfWorker != value)
                {
                    titleOfLineChartOfWorker = value;
                    NotifyPropertyChanged("TitleOfLineChartOfWorkerProperty");
                }                
            }
        }



        private DateTime startDateInDatePicker = default;

        public DateTime StartDateInDatePickerProperty
        {
            get { return startDateInDatePicker; }
            set 
            {
                if (!startDateInDatePicker.Date.Equals(value.Date))
                {
                    startDateInDatePicker = value;
                    NotifyPropertyChanged("StartDateInDatePickerProperty");
                }                
            }
        }

        private DateTime selectedDateStartInDatePicker = default;

        public DateTime SelectedDateStartInDatePickerProperty
        {
            get { return selectedDateStartInDatePicker; }
            set
            {
                if (!selectedDateStartInDatePicker.Date.Equals(value.Date))
                {
                    selectedDateStartInDatePicker = value;
                }
            }
        }

        private DateTime selectedDateEndInDatePicker = default;

        public DateTime SelectedDateEndInDatePickerProperty
        {
            get { return selectedDateEndInDatePicker; }
            set
            {
                if (!selectedDateEndInDatePicker.Date.Equals(value.Date))
                {
                    selectedDateEndInDatePicker = value;
                }
            }
        }

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
        public static bool AreTablesTheSame(DataTable tbl1, DataTable tbl2)
        {
            if (tbl1.Rows.Count != tbl2.Rows.Count || tbl1.Columns.Count != tbl2.Columns.Count)
                return false;

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

        public WpfDatabase()
        {
            connectionString = "Server=tcp:mysqlservercovid.database.windows.net,1433;Initial Catalog=myCovidKeeper;Persist Security Info=False;User ID=azureuser;Password=Amitai5925;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
        }
        public void NotifyPropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        public List<object[]> QuerySelectOfMultiRows(string selectQuery)
        {
            List<object[]> result_list = new List<object[]>();
            using (var command = sqlConnection.CreateCommand())
            {
                command.CommandText = selectQuery;
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        object[] lineInformationFromSQL = new object[reader.FieldCount];
                        reader.GetValues(lineInformationFromSQL);
                        result_list.Add(lineInformationFromSQL);
                    }
                }
            }
            return result_list;
        }

        public object[] QuerySelectOfOneRow(string selectQuery)
        {
            object[] lineInformationFromSQL;
            using (var command = sqlConnection.CreateCommand())
            {
                command.CommandText = selectQuery;
                using (var reader = command.ExecuteReader())
                {
                    lineInformationFromSQL = new object[reader.FieldCount];
                    if (reader.Read())
                    {
                        reader.GetValues(lineInformationFromSQL);
                    }
                    else
                    {
                        lineInformationFromSQL = null;
                    }
                }
            }
            return CheckIfNull<object[]>(lineInformationFromSQL);
        }

        private T CheckIfNull<T>(T valueToCheck)
        {
            if (valueToCheck == null)
            {
                return default;
            }
            return valueToCheck;
        }
        public void InsertWorker(string idWorker, string fullname, string emailAddress, BitmapImage imagePath)
        {
            using (var command = sqlConnection.CreateCommand())
            {
                byte[] imageToByte = ImagePathToByteArray(imagePath);
                string insertQuery = @"INSERT INTO [dbo].[Workers] VALUES (@Id, @Fullname, @Email_address);";
                command.CommandText = insertQuery;
                command.Parameters.AddWithValue("@Id", idWorker);
                command.Parameters.AddWithValue("@Fullname", fullname);
                command.Parameters.AddWithValue("@Email_address", emailAddress);
                command.ExecuteNonQuery();
                UploadImageToStorage(idWorker, imageToByte);
                DataTable workerDetailsTableTemp = WorkerDetailsTableProperty;
                workerDetailsTableTemp.Rows.Add(idWorker, fullname, emailAddress, imageToByte);
                WorkerDetailsTableProperty = workerDetailsTableTemp;
            }
        }

        private CloudBlobContainer GetCloudBlobContainer()
        {
            var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            var cloudBlobContainer = cloudBlobClient.GetContainerReference("pictures");
            cloudBlobContainer.CreateIfNotExists();
            cloudBlobContainer.SetPermissions(new BlobContainerPermissions() { PublicAccess = BlobContainerPublicAccessType.Blob });
            return cloudBlobContainer;
        }

        private void UploadImageToStorage(string idWorker, byte[] imageToByte)
        {
            CloudBlobContainer cloudBlobContainer = GetCloudBlobContainer();
            CloudBlockBlob blockBlob = cloudBlobContainer.GetBlockBlobReference(idWorker + ".jpg");
            using (var stream = new MemoryStream(imageToByte, writable: false))
            {
                blockBlob.UploadFromStream(stream);
            }
        }
        
        private byte[] ImagePathToByteArray(BitmapImage imagePath)
        {
            Image temp = BitmapImage2Bitmap(imagePath);
            MemoryStream strm = new MemoryStream();
            temp.Save(strm, System.Drawing.Imaging.ImageFormat.Jpeg);
            return strm.ToArray();
        }
        private Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }

        public void UpdateWorkerDetails(string idWorker, string fullname, string emailAddress, BitmapImage imagePath, int indexOfSelectedRow)
        {
            using (var command = sqlConnection.CreateCommand())
            {
                byte[] imageToByte = ImagePathToByteArray(imagePath);
                DataTable workerDetailsTableTemp = WorkerDetailsTableProperty;
                var rowToChange = workerDetailsTableTemp.Rows[indexOfSelectedRow];
                string updateQuery = @"UPDATE [dbo].[Workers] SET Fullname = @Fullname, Email_address = @Email_address Where Id = @Id;";
                command.CommandText = updateQuery;
                command.Parameters.AddWithValue("@Id", idWorker);
                command.Parameters.AddWithValue("@Fullname", fullname);
                command.Parameters.AddWithValue("@Email_address", emailAddress);
                command.ExecuteNonQuery();
                UploadImageToStorage(idWorker, imageToByte);
                rowToChange["Id"] = idWorker;
                rowToChange["Fullname"] = fullname;
                rowToChange["Email_address"] = emailAddress;
                rowToChange["Image"] = imageToByte;
                WorkerDetailsTableProperty = workerDetailsTableTemp;
            }
        }

        public void DeleteWorker(string idWorker, int indexOfSelectedRow)
        {
            using (var command = sqlConnection.CreateCommand())
            {
                string deleteQuery = @"DELETE FROM [dbo].[Workers] WHERE Id = " + idWorker + ";";
                command.CommandText = deleteQuery;
                command.ExecuteNonQuery();
                DataTable workerDetailsTableTemp = WorkerDetailsTableProperty;
                var rowToChange = workerDetailsTableTemp.Rows[indexOfSelectedRow];
                rowToChange.Delete();
                WorkerDetailsTableProperty = workerDetailsTableTemp;
            }
        }
        public void RefreshDataHome()
        {
            CountWorkersWithoutMaskToday();
            GetActiveButtonContent();
        }

        public void RefreshStatisticalData()
        {
            GetMinDateInHistoryEvents();
        }
        private void InitialWorkWeekAndAmountEventsDict()
        {
            // Gets the Calendar instance associated with a CultureInfo.
            CultureInfo myCI = new CultureInfo("en-US");
            Calendar myCal = myCI.Calendar;

            // Gets the DTFI properties required by GetWeekOfYear.
            CalendarWeekRule myCWR = myCI.DateTimeFormat.CalendarWeekRule;
            DayOfWeek myFirstDOW = myCI.DateTimeFormat.FirstDayOfWeek;
            int firstWorkWeekWithEvents = myCal.GetWeekOfYear(StartDateInDatePickerProperty, myCWR, myFirstDOW);
            int lastWorkWeek = myCal.GetWeekOfYear(DateTime.Now, myCWR, myFirstDOW);
            /*if ((int) DateTime.Now.DayOfWeek < 5)
            {
                lastWorkWeek--;
            }*/
            for (int i = StartDateInDatePickerProperty.Year; i <= DateTime.Now.Year; i++)
            {
                DateTime LastDay = new System.DateTime(i, 12, 31);
                int weekAmount = myCal.GetWeekOfYear(LastDay, myCWR, myFirstDOW);
                int firstWorkWeek = 1;
                if (i == StartDateInDatePickerProperty.Year)
                {
                    firstWorkWeek = firstWorkWeekWithEvents;
                }
                if (i == DateTime.Now.Year)
                {
                    weekAmount = lastWorkWeek;
                }
                for (; firstWorkWeek <= weekAmount; firstWorkWeek++)
                {
                    workWeekAndAmountEventsDict.Add(firstWorkWeek + "-" + i, 0);
                }
            }
        }
        private void InitialMonthAndAmountEventsDict()
        {
            // Uses the default calendar of the InvariantCulture.
            Calendar myCal = CultureInfo.InvariantCulture.Calendar;
            int firstMonthWithEvents = myCal.GetMonth(StartDateInDatePickerProperty);
            int lastMonth = myCal.GetMonth(DateTime.Now);
            for (int i = StartDateInDatePickerProperty.Year; i <= DateTime.Now.Year; i++)
            {
                int monthAmount = myCal.GetMonthsInYear(i);
                int firstMonth = 1;
                if (i == StartDateInDatePickerProperty.Year)
                {
                    firstMonth = firstMonthWithEvents;
                }
                if (i == DateTime.Now.Year)
                {
                    monthAmount = lastMonth;
                }
                for (; firstMonth <= monthAmount; firstMonth++)
                {
                    monthAndAmountEventsDict.Add(((MonthEnumForGraphs) firstMonth).ToString() + "-" + i, 0);
                }
            }
        }
        private void GetMinDateInHistoryEvents()
        {
            if (StartDateInDatePickerProperty.Equals(default))
            {
                string minDateQuery = "select min(Time_of_event) from [dbo].[History_Events]";
                object[] minDate = QuerySelectOfOneRow(minDateQuery);
                if (minDate != null)
                {
                    StartDateInDatePickerProperty = Convert.ToDateTime(minDate[0]);
                }
                InitialWorkWeekAndAmountEventsDict();
                InitialMonthAndAmountEventsDict();
            }
        }

        private void GetActiveButtonContent()
        {
            string handleQuery = "SELECT Handle from [dbo].[Starter]";
            object[] handle = QuerySelectOfOneRow(handleQuery);
            if (handle != null)
            {
                StartOrClose startOrClose = (StartOrClose)Enum.Parse(typeof(StartOrClose), handle[0].ToString());
                ActiveButtonContentProperty = startOrClose;
            }
        }

        public void CountWorkersWithoutMaskToday()
        {
            string countQuery = "SELECT COUNT( DISTINCT[Id_worker]) as num_bad_workers FROM[dbo].[History_Events] WHERE DATEDIFF(day, Time_of_event, GETDATE())<= 0;";
            object[] counter = QuerySelectOfOneRow(countQuery);
            if (counter != null)
            {
                string countAllWorkers = CountWorkers();
                if (countAllWorkers != null)
                {
                    HowManyWorkersWithoutMaskProperty = counter[0].ToString() + "/" + countAllWorkers;
                }
                
            }
        }
        private string CountWorkers()
        {
            string countQuery = "SELECT Count(Id) FROM [dbo].[Workers];";
            object[] counter = QuerySelectOfOneRow(countQuery);
            if (counter != null)
            {
                return counter[0].ToString();
            }
            return null;
        }
        public void RefreshDataWorkersTable()
        {
            GetWorkersDetailsTemp();
        }

        private byte[] GetImageWorker(string idWorker, CloudBlobContainer cloudBlobContainer=default)
        {
            if (cloudBlobContainer == default)
            {
                cloudBlobContainer = GetCloudBlobContainer();
            }
            CloudBlockBlob blockBlob = cloudBlobContainer.GetBlockBlobReference(idWorker + ".jpg");
            using (var memoryStream = new MemoryStream())
            {
                blockBlob.DownloadToStream(memoryStream);
                return memoryStream.ToArray();
            }
        }
        public void GetWorkersDetailsTemp()
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = "Select * From [dbo].[Workers]";
            command.Connection = sqlConnection;
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
            DataTable dataTableImages = new DataTable("Workers");
            sqlDataAdapter.Fill(dataTableImages);
            dataTableImages.Columns.Add("Image", typeof(byte[]));

            CloudBlobContainer cloudBlobContainer = GetCloudBlobContainer(); 
            foreach (DataRow row in dataTableImages.Rows)
            {
                string idWorker = row[0].ToString();
                row["Image"] = GetImageWorker(idWorker, cloudBlobContainer);
                /*CloudBlockBlob blockBlob = cloudBlobContainer.GetBlockBlobReference(idWorker + ".jpg");
                using (var memoryStream = new MemoryStream())
                {
                    blockBlob.DownloadToStream(memoryStream);
                    row["Image"] = memoryStream.ToArray();
                }*/
            }            
            WorkerDetailsTableProperty = dataTableImages;
        }
        public void PercentageWorkersWithoutMaskTodayPerYesterday()
        {
            string todayPerYesterdayQuery = "select " +
                "(SELECT COUNT( DISTINCT[Id_worker]) as num_bad_workers " +
                "FROM[dbo].[History_Events] " +
                "WHERE DATEDIFF(day, Time_of_event, GETDATE())<= 0) as countToday, " +
                "(SELECT COUNT(DISTINCT[Id_worker]) as num_bad_workers " +
                "FROM[dbo].[History_Events] " +
                "WHERE DATEDIFF(day, Time_of_event, GETDATE()-1)<= 0) as countYesterday; ";
            object[] todayPerYesterday = QuerySelectOfOneRow(todayPerYesterdayQuery);
            if (todayPerYesterday != null)
            {
                int countToday = Convert.ToInt32(todayPerYesterday[0]);
                int countYesterday = Convert.ToInt32(todayPerYesterday[1]);
                if (countYesterday != 0)
                {
                    PercentageWorkersWithoutMaskTodayPerYesterdayProperty = (countToday / countYesterday) * 100 - 100;
                }
            }
        }
        private DataTable SearchTableByQuery(string query)
        {
            DataRow[] results = WorkerDetailsTableProperty.Select(query);
            DataTable dataTable = new DataTable("Workers");
            dataTable.Columns.Add("Id", typeof(String));
            dataTable.Columns.Add("FullName", typeof(String));
            dataTable.Columns.Add("Email_address", typeof(String));
            dataTable.Columns.Add("Image", typeof(byte[]));
            foreach (DataRow row in results)
            {
                dataTable.ImportRow(row);
            }
            return dataTable;
        }
        public void SearchById(string idWorker)
        {
            /*DataRow[] results = WorkerDetailsTableProperty.Select("Id = '" + idWorker + "'");
            DataTable dataTable = new DataTable("Workers");
            dataTable.Columns.Add("Id", typeof(String));
            dataTable.Columns.Add("FullName", typeof(String));
            dataTable.Columns.Add("Email_address", typeof(String));
            dataTable.Columns.Add("Image", typeof(byte[]));
            foreach (DataRow row in results)
            {
                dataTable.ImportRow(row);
            }*/
            SearchWorkerDetailsTableProperty = SearchTableByQuery("Id = '" + idWorker + "'");
        }
        public void SearchByIdAndEmail(string idWorker, string emailAddress)
        {
            /*DataRow[] results = WorkerDetailsTableProperty.Select("Id = '" + idWorker + "' AND Email_address = '" + emailAddress + "'");
            DataTable dataTable = new DataTable("Workers");
            dataTable.Columns.Add("Id", typeof(String));
            dataTable.Columns.Add("FullName", typeof(String));
            dataTable.Columns.Add("Email_address", typeof(String));
            dataTable.Columns.Add("Image", typeof(byte[]));
            foreach (DataRow row in results)
            {
                dataTable.ImportRow(row);
            }*/
            SearchWorkerDetailsTableProperty = SearchTableByQuery("Id = '" + idWorker + "' AND Email_address = '" + emailAddress + "'");
        }
        public void SearchByIdAndFullName(string idWorker, string fullName)
        {
            /*DataRow[] results = WorkerDetailsTableProperty.Select("Id = '" + idWorker + "' AND FullName = '" + fullName + "'");
            DataTable dataTable = new DataTable("Workers");
            dataTable.Columns.Add("Id", typeof(String));
            dataTable.Columns.Add("FullName", typeof(String));
            dataTable.Columns.Add("Email_address", typeof(String));
            dataTable.Columns.Add("Image", typeof(byte[]));
            foreach (DataRow row in results)
            {
                dataTable.ImportRow(row);
            }*/
            SearchWorkerDetailsTableProperty = SearchTableByQuery("Id = '" + idWorker + "' AND FullName = '" + fullName + "'");
        }
        public void SearchByFullName(string fullName)
        {
            /*DataRow[] results = WorkerDetailsTableProperty.Select("FullName = '" + fullName + "'");
            DataTable dataTable = new DataTable("Workers");
            dataTable.Columns.Add("Id", typeof(String));
            dataTable.Columns.Add("FullName", typeof(String));
            dataTable.Columns.Add("Email_address", typeof(String));
            dataTable.Columns.Add("Image", typeof(byte[]));
            foreach (DataRow row in results)
            {
                dataTable.ImportRow(row);
            }*/
            SearchWorkerDetailsTableProperty = SearchTableByQuery("FullName = '" + fullName + "'");
        }
        public void SearchByFullNameAndEmail(string fullName, string emailAddress)
        {
            /*DataRow[] results = WorkerDetailsTableProperty.Select("FullName = '" + fullName + "' AND Email_address = '" + emailAddress + "'");
            DataTable dataTable = new DataTable("Workers");
            dataTable.Columns.Add("Id", typeof(String));
            dataTable.Columns.Add("FullName", typeof(String));
            dataTable.Columns.Add("Email_address", typeof(String));
            dataTable.Columns.Add("Image", typeof(byte[]));
            foreach (DataRow row in results)
            {
                dataTable.ImportRow(row);
            }*/
            SearchWorkerDetailsTableProperty = SearchTableByQuery("FullName = '" + fullName + "' AND Email_address = '" + emailAddress + "'");
        }
        public void SearchByEmail(string emailAddress)
        {
            /*DataRow[] results = WorkerDetailsTableProperty.Select("Email_address = '" + emailAddress + "'");
            DataTable dataTable = new DataTable("Workers");
            dataTable.Columns.Add("Id", typeof(String));
            dataTable.Columns.Add("FullName", typeof(String));
            dataTable.Columns.Add("Email_address", typeof(String));
            dataTable.Columns.Add("Image", typeof(byte[]));
            foreach (DataRow row in results)
            {
                dataTable.ImportRow(row);
            }*/
            SearchWorkerDetailsTableProperty = SearchTableByQuery("Email_address = '" + emailAddress + "'");
        }
        public void SearchByIdAndFullNameAndEmail(string idWorker, string fullName, string emailAddress)
        {
            /*DataRow[] results = WorkerDetailsTableProperty.Select("Id = '" + idWorker + "' AND FullName = '" + fullName + "' AND Email_address = '" + emailAddress + "'");
            DataTable dataTable = new DataTable("Workers");
            dataTable.Columns.Add("Id", typeof(String));
            dataTable.Columns.Add("FullName", typeof(String));
            dataTable.Columns.Add("Email_address", typeof(String));
            dataTable.Columns.Add("Image", typeof(byte[]));
            foreach (DataRow row in results)
            {
                dataTable.ImportRow(row);
            }*/
            SearchWorkerDetailsTableProperty = SearchTableByQuery("Id = '" + idWorker + "' AND FullName = '" + fullName + "' AND Email_address = '" + emailAddress + "'");
        }

        public void StartOrCloseProgram()
        {            
            if (ActiveButtonContentProperty == StartOrClose.Start)
            {
                UpdateHandleInStarter("1", "0");
                ActiveButtonContentProperty = StartOrClose.Close;
            }
            else
            {
                UpdateHandleInStarter("0", "1");
                ActiveButtonContentProperty = StartOrClose.Start;
            }
        }

        private void UpdateHandleInStarter(string valueInTable, string valueToUpdate)
        {
            using (var command = sqlConnection.CreateCommand())
            {
                string updateQuery = @"UPDATE [dbo].[Starter] SET Handle = @Handle where Handle = " + valueInTable;
                command.CommandText = updateQuery;
                command.Parameters.AddWithValue("@Handle", valueToUpdate);
                command.ExecuteNonQuery();
            }
        }
        private void RefreshSearchWorkers()
        {
            SearchWorkerDetailsTableProperty = default;
        }

        private List<GraphContent> ConvertDictToListGraphContent(Dictionary<string, double> dictToListGraphContent)
        {
            List<GraphContent> avgEvents = new List<GraphContent>();
            foreach (var item in dictToListGraphContent)
            {
                avgEvents.Add(new GraphContent() { WorkWeek = item.Key, AvgValue = item.Value });
            }
            return avgEvents;
        }

        public void GetAvgEventsPerWeek()
        {
            string avgEventsPerWeekQuery = "Select DISTINCT DATEPART(WEEK, Time_of_event) AS WW, DATEPART(year, Time_of_event) AS Year, " +
                "COUNT(CONVERT(date, Time_of_event)) / 5.0 AS Avg " +
                "from[dbo].[History_Events] " +
                "group by DATEPART(WEEK, Time_of_event), DATEPART(year, Time_of_event); ";
            List<object[]> avgEventsPerWeekList = QuerySelectOfMultiRows(avgEventsPerWeekQuery);
            
            if (avgEventsPerWeekList != null)
            {
                SetValuesInDictToZero(this.workWeekAndAmountEventsDict);
                foreach (var item in avgEventsPerWeekList)
                {
                    string weekStr = item[0].ToString() + "-" + item[1].ToString();
                    this.workWeekAndAmountEventsDict[weekStr] = Convert.ToDouble(item[2]);
                }
                ColumnGraphProperty = ConvertDictToListGraphContent(this.workWeekAndAmountEventsDict);
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
        public void GetAmountEventsByWorker()
        {
            string amountEventsPerWorkerQuery = "Select DISTINCT Id_worker, FullName, " +
                "COUNT(Id_worker) AS Count " +
                "from[dbo].[History_Events] " +
                "INNER JOIN [dbo].[Workers] ON [dbo].[History_Events].Id_worker=[dbo].[Workers].Id " +
                "group by Id_worker, FullName " +
                "order by count desc; ";
            SqlCommand command = new SqlCommand();
            command.CommandText = amountEventsPerWorkerQuery;
            command.Connection = sqlConnection;
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
            DataTable dataTableAmountEventsPerWorker = new DataTable("History_Events");
            sqlDataAdapter.Fill(dataTableAmountEventsPerWorker);
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
                /*TitleOfLineChartOfWorkerProperty = WeekOrMonth.Week;
                LineGraphOfWorkerProperty = this.workWeekAndAmountEventsDict;*/
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
        public void GetAvgEventsPerMonthPerWorker(string idWorker)
        {
            string avgEventsPerMonthPerWorkerQuery = "Select DISTINCT DATEPART(month, Time_of_event) AS Month, " +
                "COUNT(CONVERT(date, Time_of_event)) AS Avg " +
                "from[dbo].[History_Events] " +
                "where Id_worker = " + idWorker +" " +
                "group by DATEPART(month, Time_of_event); ";
            List<object[]> avgEventsPerMonthPerWorkerList = QuerySelectOfMultiRows(avgEventsPerMonthPerWorkerQuery);

            if (avgEventsPerMonthPerWorkerList != null)
            {
                List<KeyValuePair<string, double>> avgEventsPerMonthPerWorker = new List<KeyValuePair<string, double>>();
                foreach (var item in avgEventsPerMonthPerWorkerList)
                {
                    avgEventsPerMonthPerWorker.Add(new KeyValuePair<string, double>(item[0].ToString(), Convert.ToDouble(item[1])));
                }
                TitleOfLineChartOfWorkerProperty = WeekOrMonth.Week;
                //GraphProperty = avgEventsPerMonthPerWorker.ToArray();
            }
        }

        private Dictionary<string,double> SetValuesInDictToZero(Dictionary<string, double> dictToSet)
        {
            foreach (var key in this.workWeekAndAmountEventsDict.Keys.ToList())
            {
                dictToSet[key] = 0;
            }
            return dictToSet;
        }

    }
}
