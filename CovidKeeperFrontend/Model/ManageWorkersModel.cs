using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace CovidKeeperFrontend.Model
{
    //This class is the model of HomeUserControl
    public class ManageWorkersModel : AbstractModel
    {
        
        public NotifyTaskCompletion<int> UpdateHandleInAnalayzerConfigAsync { get; private set; }
        //Property that defines the workers details table
        private DataTable workerDetailsTable = default;
        public DataTable WorkerDetailsTableProperty
        {
            get { return workerDetailsTable; }
            set
            {
                if (workerDetailsTable != value)
                {
                    workerDetailsTable = value;
                    //CountWorkersInWorkersDetailsTableProperty = value.Rows.Count.ToString();
                    //Update the CountWorkersInWorkersDetailsTableProperty with the number of rows of this dataTable
                    CountWorkersInWorkersDetailsTableProperty = value.Rows.Count.ToString();
                    //Update the flag of UpdateHandleInAnalayzerConfig
                    UpdateHandleInAnalayzerConfigAsync = new NotifyTaskCompletion<int>(UpdateHandleInAnalayzerConfig());
                    //This is WorkersDetailsTableProperty and not SearchWorkerDetailsTableProperty
                    SearchOrWorkersTableProperty = false;
                    NotifyPropertyChanged("WorkerDetailsTableProperty");
                }
            }
        }

        //Property that defines the SearchWorkerDetailsTableProperty when the client wants to search workers
        private DataTable searchWorkerDetailsTable = default;
        public DataTable SearchWorkerDetailsTableProperty
        {
            get { return searchWorkerDetailsTable; }
            set
            {
                if (searchWorkerDetailsTable != value)
                {
                    searchWorkerDetailsTable = value;
                    //Update the CountWorkersInWorkersDetailsTableProperty with the number of rows of this dataTable
                    CountWorkersInWorkersDetailsTableProperty = value.Rows.Count.ToString();
                    //This is SearchWorkerDetailsTableProperty and not WorkersDetailsTableProperty
                    SearchOrWorkersTableProperty = true;
                    NotifyPropertyChanged("WorkerDetailsTableProperty");
                }
            }
        }

        //Property that defines if we need to show the client the SerachTable or the WorkersDetailsTable
        private bool searchOrWorkersTable = false;
        public bool SearchOrWorkersTableProperty
        {
            get { return searchOrWorkersTable; }
            set { searchOrWorkersTable = value; }
        }

        //Property that defines the count workers in WorkersDetailsTable
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

        public NotifyTaskCompletion<int> RefreshData { get; private set; }
        public ManageWorkersModel()
        {
            //Refresh data
            RefreshData = new NotifyTaskCompletion<int>(RefreshDataAsync());
        }

        //Override this function to refresh the data in the ManageWorkersUserControl
        public override async Task<int> RefreshDataAsync()
        {
            await GetWorkersDetailsTable();
            return 1;
        }

        //Function that return DataTable that is part of WorkerDetailsTableProperty according to the given query
        private DataTable SearchTableByQuery(string query)
        {
            WorkerDetailsTableProperty.CaseSensitive = false;
            DataRow[] results = WorkerDetailsTableProperty.Select(query);
            WorkerDetailsTableProperty.CaseSensitive = true;
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

        //Function that reponsible for searching worker by the given id
        public void SearchById(string idWorker)
        {
            SearchWorkerDetailsTableProperty = SearchTableByQuery("Id like '%" + idWorker + "%'");
        }

        //Function that reponsible for searching worker by the given fullName
        public void SearchByFullName(string fullName)
        {
            SearchWorkerDetailsTableProperty = SearchTableByQuery("FullName like '%" + fullName + "%'");
        }

        //Function that reponsible for searching worker by the given email address
        public void SearchByEmail(string emailAddress)
        {
            SearchWorkerDetailsTableProperty = SearchTableByQuery("Email_address like '%" + emailAddress + "%'");
        }

        //Function that gets the the workers details table
        public async Task GetWorkersDetailsTable()
        {
            await Task.Run(() =>
            {
                DataTable dataTableImages = GetDataTableByQuery("Select * From [dbo].[Workers]", "Workers");
                dataTableImages.Columns.Add("Image", typeof(byte[]));
                dataTableImages.Columns.Add("Image_checkbox", typeof(bool));
                foreach (DataRow row in dataTableImages.Rows)
                {
                    string idWorker = row[0].ToString();
                    row["Image"] = GetImageWorker(idWorker);
                    row["Image_checkbox"] = true;
                }
                WorkerDetailsTableProperty = dataTableImages;
            });            
        }

        //Function that updates to 1 the Analayzer's flag in the azure database
        private async Task<int> UpdateHandleInAnalayzerConfig()
        {
            string updateQuery = @"UPDATE [dbo].[Analayzer_config] SET Handle = @Handle";
            Dictionary<string, string> fieldNameToValueDict = new Dictionary<string, string>
            {
                { "@Handle", "1" }
            };
            await QueryDatabaseWithDict(updateQuery, fieldNameToValueDict);
            return default;
        }

        //Function that insert worker to azure database and updating the WorkerDetailsTableProperty
        public async Task InsertWorker(string idWorker, string fullname, string emailAddress, BitmapImage imagePath)
        {
            string insertQuery = @"INSERT INTO [dbo].[Workers] VALUES (@Id, @Fullname, @Email_address);";
            Dictionary<string, string> fieldNameToValueDict = new Dictionary<string, string>
            {
                { "@Id", idWorker },
                { "@FullName", fullname },
                { "@Email_address", emailAddress }
            };
            await QueryDatabaseWithDict(insertQuery, fieldNameToValueDict);
            byte[] imageToByte = ImagePathToByteArray(imagePath);
            await UploadImageToStorage(idWorker, imageToByte);
            DataTable workerDetailsTableTemp = WorkerDetailsTableProperty;
            workerDetailsTableTemp.Rows.Add(idWorker, fullname, emailAddress, imageToByte);
            WorkerDetailsTableProperty = workerDetailsTableTemp;
        }

        //Function that converts BitmapImage to byte array
        private byte[] ImagePathToByteArray(BitmapImage imagePath)
        {
            Image temp = BitmapImage2Bitmap(imagePath);
            MemoryStream strm = new MemoryStream();
            temp.Save(strm, System.Drawing.Imaging.ImageFormat.Jpeg);
            return strm.ToArray();
        }

        //Function that converts BitmapImage to Bitmap
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

        //Function that updates the worker's details and updates the WorkerDetalisTable
        public async Task<int> UpdateWorkerDetails(string idWorkerInDataTable, string idWorker, string fullname, string emailAddress, BitmapImage imagePath, int indexOfSelectedRow)
        {
            byte[] imageToByte = ImagePathToByteArray(imagePath);
            DataTable workerDetailsTableTemp = WorkerDetailsTableProperty;
            var rowToChange = workerDetailsTableTemp.Rows[indexOfSelectedRow];
            await UploadImageToStorage(idWorker, imageToByte);
            rowToChange["Id"] = idWorker;
            rowToChange["Fullname"] = fullname;
            rowToChange["Email_address"] = emailAddress;
            rowToChange["Image"] = imageToByte;
            WorkerDetailsTableProperty = workerDetailsTableTemp;
            //The id has changed
            if (idWorkerInDataTable != idWorker)
            {
                await UpdateWorkerId(idWorkerInDataTable, idWorker, fullname, emailAddress);
            }
            //The id has not changed
            else
            {
                string updateQuery = @"UPDATE [dbo].[Workers] SET FullName = @FullName, Email_address = @Email_address Where Id = @Id;";
                Dictionary<string, string> fieldNameToValueDict = new Dictionary<string, string>
                {
                    { "@Id", idWorker },
                    { "@FullName", fullname },
                    { "@Email_address", emailAddress }
                };
                await QueryDatabaseWithDict(updateQuery, fieldNameToValueDict);                
            }            
            return default;
        }

        //Function that updates worker's details in case the worker's id has been changed
        private async Task UpdateWorkerIdToNewId(string newIdWorker, string idWorkerToUpdate, string fullname, string emailAddress)
        {
            string updateQuery = @"UPDATE [dbo].[Workers] SET Id = @Id, FullName = @FullName, Email_address = @Email_address Where Id = " + idWorkerToUpdate;
            Dictionary<string, string> fieldNameToValueDict = new Dictionary<string, string>
            {
                { "@Id", newIdWorker },
                { "@FullName", fullname },
                { "@Email_address", emailAddress }
            };
            await QueryDatabaseWithDict(updateQuery, fieldNameToValueDict);
        }

        //Function that resposible for updating worker's details in case the worker's id has been changed,
        //so we need to delete and save all the worker's events, updating worker's id and then insert the events with the new id
        private async Task UpdateWorkerId(string idWorkerInDataTable, string idWorker, string fullname, string emailAddress)
        {
            List<object[]> eventsByIdList = QuerySelectOfMultiRows("select * from [dbo].[History_Events] where Id_worker = '" + idWorkerInDataTable + "'");
            if (eventsByIdList != null)
            {
                string deleteQuery = @"DELETE FROM [dbo].[History_Events] WHERE Id_worker = '" + idWorkerInDataTable + "';";
                await QueryDatabaseWithDict(deleteQuery);
            }
            await UpdateWorkerIdToNewId(idWorker, idWorkerInDataTable, fullname, emailAddress);
            await DeleteImageFromStorage(idWorkerInDataTable);
            if (eventsByIdList != null)
            {
                await InsertEventsListById(eventsByIdList, idWorker);
            }
        }

        //Function that deletes worker and updating the WorkerDetailsTableProperty
        public async Task DeleteWorker(string idWorker, int indexOfSelectedRow=-1)
        {
            string deleteQuery = @"DELETE FROM [dbo].[Workers] WHERE Id = " + idWorker + ";";
            await QueryDatabaseWithDict(deleteQuery);
            DataTable workerDetailsTableTemp = WorkerDetailsTableProperty;
            if (indexOfSelectedRow != -1)
            {
                var rowToChange = workerDetailsTableTemp.Rows[indexOfSelectedRow];
                rowToChange.Delete();
                WorkerDetailsTableProperty = workerDetailsTableTemp;
            }            
        }

        //Function that refresh WorkerDetailsTableProperty and returns it to the ManageWorkersUserControl
        public void GetWorkersDetailsAfterRefresh()
        {
            SearchOrWorkersTableProperty = false;
            NotifyPropertyChanged("WorkerDetailsTableProperty");
        }
    }
}
