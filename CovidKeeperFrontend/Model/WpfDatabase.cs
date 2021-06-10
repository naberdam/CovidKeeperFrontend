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
        private string countWorkersInWorkersDetailsTable;

        public string CountWorkersInWorkersDetailsTableProperty
        {
            get { return countWorkersInWorkersDetailsTable; }
            set 
            {
                if (countWorkersInWorkersDetailsTable != value)
                {
                    countWorkersInWorkersDetailsTable = value;
                    NotifyPropertyChanged("CountWorkersInWorkersDetailsTableProperty");
                }                
            }
        }

        public NotifyTaskCompletion<int> UpdateHandleInAnalayzerConfigAsync { get; private set; }
        private DataTable workerDetailsTable = default;
        public DataTable WorkerDetailsTableProperty 
        {
            get { return workerDetailsTable; } 
            set
            {
                if (workerDetailsTable != value)
                {
                    workerDetailsTable = value;
                    CountWorkersInWorkersDetailsTableProperty = value.Rows.Count.ToString();
                    UpdateHandleInAnalayzerConfigAsync = new NotifyTaskCompletion<int>(UpdateHandleInAnalayzerConfig());
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
        public enum HandleFlag
        {
            Close = 1,
            Start = 0
        }

        private HandleFlag activeButtonContent;

        public HandleFlag ActiveButtonContentProperty
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

        public NotifyTaskCompletion<int> RefreshUserControlAsync { get; private set; }
        private int indexOfMenuList = -1;
        public int IndexOfMenuListProperty
        {
            set 
            {
                // The first time the client entered the program
                if (indexOfMenuList == -1)
                {
                    RefreshDataWorkersTable();
                    RefreshUserControlAsync = new NotifyTaskCompletion<int>(RefreshDataHome());                    
                    indexOfMenuList = value;
                    return;
                }
                if (indexOfMenuList != value)
                {
                    switch (value)
                    {
                        case 0:
                            RefreshUserControlAsync = new NotifyTaskCompletion<int>(RefreshDataHome());
                            break;
                        case 1:
                            break;
                        case 2:
                            RefreshUserControlAsync = new NotifyTaskCompletion<int>(RefreshStatisticalData());
                            //RefreshSearchWorkers();
                            break;
                        case 3:
                            
                            break;
                        case 4:
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
                if (amountEventsByWorkerTable == default || !AreTablesTheSame(amountEventsByWorkerTable, value))
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

        public NotifyTaskCompletion<int> UpdateTimeBreakForMailsAsync { get; private set; }
        private int minutesBreakForMails;
        public int MinutesBreakForMailsProperty
        {
            get { return minutesBreakForMails; }
            set 
            {
                if (minutesBreakForMails != value)
                {
                    minutesBreakForMails = value;
                    UpdateTimeBreakForMailsAsync = new NotifyTaskCompletion<int>(UpdateTimeBreakForMails(minutesBreakForMails));                   
                }
            }
        }

        public WpfDatabase()
        {
            connectionString = "Server=tcp:mysqlservercovid.database.windows.net,1433;" +
                "Initial Catalog=myCovidKeeper;" +
                "Persist Security Info=False;" +
                "User ID=azureuser;" +
                "Password=Amitai5925;" +
                "MultipleActiveResultSets=True;" +
                "Encrypt=True;" +
                "TrustServerCertificate=False;Connection Timeout=30;";
            sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            object[] result = QuerySelectOfOneRow("select Handle from [dbo].[Starter]");
            HandleFlag starterHandleEnum = (HandleFlag)Enum.Parse(typeof(HandleFlag), result[0].ToString());
            ActiveButtonContentProperty = starterHandleEnum;
            RefreshUserControlAsync = new NotifyTaskCompletion<int>(RefreshDataHome());
        }
        private async Task<int> UpdateTimeBreakForMails(int minutesBreakToChange)
        {
            using (var command = sqlConnection.CreateCommand())
            {
                string updateQuery = @"UPDATE [dbo].[Manager_Config] SET Minutes_between_mails = @Minutes_between_mails, Handle = @Handle";
                command.CommandText = updateQuery;
                command.Parameters.AddWithValue("@Minutes_between_mails", minutesBreakToChange);
                command.Parameters.AddWithValue("@Handle", 1);
                await command.ExecuteNonQueryAsync();
            }
            return default;
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
        public async Task InsertWorker(string idWorker, string fullname, string emailAddress, BitmapImage imagePath)
        {
            using (var command = sqlConnection.CreateCommand())
            {
                byte[] imageToByte = ImagePathToByteArray(imagePath);
                string insertQuery = @"INSERT INTO [dbo].[Workers] VALUES (@Id, @Fullname, @Email_address);";
                command.CommandText = insertQuery;
                command.Parameters.AddWithValue("@Id", idWorker);
                command.Parameters.AddWithValue("@Fullname", fullname);
                command.Parameters.AddWithValue("@Email_address", emailAddress);
                await command.ExecuteNonQueryAsync();
                await UploadImageToStorage(idWorker, imageToByte);
                DataTable workerDetailsTableTemp = WorkerDetailsTableProperty;
                workerDetailsTableTemp.Rows.Add(idWorker, fullname, emailAddress, imageToByte);
                WorkerDetailsTableProperty = workerDetailsTableTemp;
            }
        }

        private CloudBlobContainer GetCloudBlobContainer()
        {
            /*StorageCredentials storageCredentials = new StorageCredentials("faceimages2", );
            CloudStorageAccount cloudStorageAccount = new CloudStorageAccount(storageCredentials, useHttps: true);
*/
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("pictures");
            cloudBlobContainer.CreateIfNotExists();
            cloudBlobContainer.SetPermissions(new BlobContainerPermissions() { PublicAccess = BlobContainerPublicAccessType.Blob });
            return cloudBlobContainer;
        }

        private async Task UploadImageToStorage(string idWorker, byte[] imageToByte)
        {
            CloudBlobContainer cloudBlobContainer = GetCloudBlobContainer();
            CloudBlockBlob blockBlob = cloudBlobContainer.GetBlockBlobReference(idWorker + ".jpg");
            using (var stream = new MemoryStream(imageToByte, writable: false))
            {
                await blockBlob.UploadFromStreamAsync(stream);
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

        public async Task<int> UpdateWorkerDetails(string idWorker, string fullname, string emailAddress, BitmapImage imagePath, int indexOfSelectedRow)
        {
            using (var command = sqlConnection.CreateCommand())
            {
                byte[] imageToByte = ImagePathToByteArray(imagePath);
                DataTable workerDetailsTableTemp = WorkerDetailsTableProperty;
                var rowToChange = workerDetailsTableTemp.Rows[indexOfSelectedRow];
                string updateQuery = @"UPDATE [dbo].[Workers] SET FullName = @FullName, Email_address = @Email_address Where Id = @Id;";
                command.CommandText = updateQuery;
                command.Parameters.AddWithValue("@Id", idWorker);
                command.Parameters.AddWithValue("@FullName", fullname);
                command.Parameters.AddWithValue("@Email_address", emailAddress);
                await command.ExecuteNonQueryAsync();
                await UploadImageToStorage(idWorker, imageToByte);
                rowToChange["Id"] = idWorker;
                rowToChange["Fullname"] = fullname;
                rowToChange["Email_address"] = emailAddress;
                rowToChange["Image"] = imageToByte;
                WorkerDetailsTableProperty = workerDetailsTableTemp;
            }
            return default;
        }

        public async Task DeleteWorker(string idWorker, int indexOfSelectedRow)
        {
            using (var command = sqlConnection.CreateCommand())
            {
                string deleteQuery = @"DELETE FROM [dbo].[Workers] WHERE Id = " + idWorker + ";";
                command.CommandText = deleteQuery;
                await command.ExecuteNonQueryAsync();
                DataTable workerDetailsTableTemp = WorkerDetailsTableProperty;
                var rowToChange = workerDetailsTableTemp.Rows[indexOfSelectedRow];
                rowToChange.Delete();
                WorkerDetailsTableProperty = workerDetailsTableTemp;
            }
        }
        public async Task<int> RefreshDataHome()
        {
            await CountWorkersWithoutMaskToday();
            await GetActiveButtonContent();
            await PercentageWorkersWithoutMaskTodayPerYesterday();
            return default;
        }

        public async Task<int> RefreshStatisticalData()
        {
            await GetMinDateInHistoryEvents();
            return default;
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
        private async Task GetMinDateInHistoryEvents()
        {
            await Task.Run(() =>
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
            });            
        }

        private async Task GetActiveButtonContent()
        {
            await Task.Run(() =>
            {
                string handleQuery = "SELECT Handle from [dbo].[Starter]";
                object[] handle = QuerySelectOfOneRow(handleQuery);
                if (handle != null)
                {
                    HandleFlag startOrClose = (HandleFlag)Enum.Parse(typeof(HandleFlag), handle[0].ToString());
                    ActiveButtonContentProperty = startOrClose;
                }
            });            
        }

        public async Task CountWorkersWithoutMaskToday()
        {
            string countAllWorkers = await CountWorkers();
            await Task.Run(() =>
            {
                string countQuery = "SELECT COUNT( DISTINCT[Id_worker]) as num_bad_workers FROM[dbo].[History_Events] WHERE DATEDIFF(day, Time_of_event, GETDATE())<= 0;";
                object[] counter = QuerySelectOfOneRow(countQuery);
                if (counter != null)
                {                    
                    if (countAllWorkers != null)
                    {
                        HowManyWorkersWithoutMaskProperty = counter[0].ToString() + "/" + countAllWorkers;
                    }
                }
            });            
        }
        private async Task<string> CountWorkers()
        {
            return await Task.Run(() =>
            {
                string countQuery = "SELECT Count(Id) FROM [dbo].[Workers];";
                object[] counter = QuerySelectOfOneRow(countQuery);
                if (counter != null)
                {
                    return counter[0].ToString();
                }
                return null;
            });
            
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
            dataTableImages.Columns.Add("Image_checkbox", typeof(bool));

            CloudBlobContainer cloudBlobContainer = GetCloudBlobContainer();
            foreach (DataRow row in dataTableImages.Rows)
            {
                string idWorker = row[0].ToString();
                row["Image"] = GetImageWorker(idWorker, cloudBlobContainer);
                row["Image_checkbox"] = true;
            }
            WorkerDetailsTableProperty = dataTableImages;
        }
        public async Task PercentageWorkersWithoutMaskTodayPerYesterday()
        {
            await Task.Run(() =>
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
            });            
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
            SearchWorkerDetailsTableProperty = SearchTableByQuery("Id = '" + idWorker + "'");
        }
        public void SearchByIdAndEmail(string idWorker, string emailAddress)
        {
            SearchWorkerDetailsTableProperty = SearchTableByQuery("Id = '" + idWorker + "' AND Email_address = '" + emailAddress + "'");
        }
        public void SearchByIdAndFullName(string idWorker, string fullName)
        {
            SearchWorkerDetailsTableProperty = SearchTableByQuery("Id = '" + idWorker + "' AND FullName = '" + fullName + "'");
        }
        public void SearchByFullName(string fullName)
        {
            SearchWorkerDetailsTableProperty = SearchTableByQuery("FullName = '" + fullName + "'");
        }
        public void SearchByFullNameAndEmail(string fullName, string emailAddress)
        {
            SearchWorkerDetailsTableProperty = SearchTableByQuery("FullName = '" + fullName + "' AND Email_address = '" + emailAddress + "'");
        }
        public void SearchByEmail(string emailAddress)
        {
            SearchWorkerDetailsTableProperty = SearchTableByQuery("Email_address = '" + emailAddress + "'");
        }
        public void SearchByIdAndFullNameAndEmail(string idWorker, string fullName, string emailAddress)
        {
            SearchWorkerDetailsTableProperty = SearchTableByQuery("Id = '" + idWorker + "' AND FullName = '" + fullName + "' AND Email_address = '" + emailAddress + "'");
        }

        public async Task StartOrCloseProgram()
        {            
            if (ActiveButtonContentProperty == HandleFlag.Start)
            {
                await UpdateHandleInStarter("0", "1");
                ActiveButtonContentProperty = HandleFlag.Close;
            }
            else
            {
                await UpdateHandleInStarter("1", "0");
                ActiveButtonContentProperty = HandleFlag.Start;
            }
        }

        private async Task UpdateHandleInStarter(string valueInTable, string valueToUpdate)
        {
            using (var command = sqlConnection.CreateCommand())
            {
                string updateQuery = @"UPDATE [dbo].[Starter] SET Handle = @Handle where Handle = " + valueInTable;
                command.CommandText = updateQuery;
                command.Parameters.AddWithValue("@Handle", valueToUpdate);
                await command.ExecuteNonQueryAsync();
            }
        }
        private async Task<int> UpdateHandleInAnalayzerConfig()
        {
            using (var command = sqlConnection.CreateCommand())
            {
                string updateQuery = @"UPDATE [dbo].[Analayzer_config] SET Handle = @Handle";
                command.CommandText = updateQuery;
                command.Parameters.AddWithValue("@Handle", "1");
                await command.ExecuteNonQueryAsync();
            }
            return default;
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

        public void GetAmountEventsByWorkerWithRange(DateTime startDate, DateTime endDate)
        {
            string amountEventsPerWorkerQuery = "Select DISTINCT Id_worker, FullName, " +
                "COUNT(Id_worker) AS Count " +
                "from[dbo].[History_Events] " +
                "INNER JOIN [dbo].[Workers] ON [dbo].[History_Events].Id_worker=[dbo].[Workers].Id " +
                "where (Time_of_event > '" + startDate.ToString("MM/dd/yyyy") + "' and Time_of_event < '" + endDate.ToString("MM/dd/yyyy") + "') " +
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
        /*public void GetAvgEventsPerMonthPerWorker(string idWorker)
        {
            string avgEventsPerMonthPerWorkerQuery = "Select DISTINCT DATEPART(month, Time_of_event) AS Month, " +
                "COUNT(CONVERT(date, Time_of_event)) AS Avg " +
                "from[dbo].[History_Events] " +
                "where Id_worker = " + idWorker + " " +
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
            }
        }*/

        private Dictionary<string,double> SetValuesInDictToZero(Dictionary<string, double> dictToSet)
        {
            foreach (var key in dictToSet.Keys.ToList())
            {
                dictToSet[key] = 0;
            }
            return dictToSet;
        }

    }
}
