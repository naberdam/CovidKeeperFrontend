using CovidKeeperFrontend.HelperClasses;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
            DataTable dataTable = new DataTable(GlobalVariables.WORKERS_TABLE_NAME);
            dataTable.Columns.Add(GlobalVariables.ID_FIELD, typeof(String));
            dataTable.Columns.Add(GlobalVariables.FULL_NAME_FIELD, typeof(String));
            dataTable.Columns.Add(GlobalVariables.EMAIL_ADDRESS_FIELD, typeof(String));
            dataTable.Columns.Add(GlobalVariables.IMAGE_FIELD, typeof(byte[]));
            foreach (DataRow row in results)
            {
                dataTable.ImportRow(row);
            }
            return dataTable;
        }

        //Function that reponsible for searching worker by the given id
        public void SearchById(string idWorker)
        {
            SearchWorkerDetailsTableProperty = SearchTableByQuery(GlobalVariables.ID_FIELD + " like '%" + idWorker + "%'");
        }

        //Function that reponsible for searching worker by the given fullName
        public void SearchByFullName(string fullName)
        {
            SearchWorkerDetailsTableProperty = SearchTableByQuery(GlobalVariables.FULL_NAME_FIELD + " like '%" + fullName + "%'");
        }

        //Function that reponsible for searching worker by the given email address
        public void SearchByEmail(string emailAddress)
        {
            SearchWorkerDetailsTableProperty = SearchTableByQuery(GlobalVariables.EMAIL_ADDRESS_FIELD + " like '%" + emailAddress + "%'");
        }

        //Function that gets the the workers details table
        public async Task GetWorkersDetailsTable()
        {
            await Task.Run(() =>
            {
                DataTable dataTableImages = GetDataTableByQuery("Select * " +
                    "From [" + GlobalVariables.DBO_NAME + "].[" + GlobalVariables.WORKERS_TABLE_NAME + "]", 
                    "" + GlobalVariables.WORKERS_TABLE_NAME + "");
                dataTableImages.Columns.Add(GlobalVariables.IMAGE_FIELD, typeof(byte[]));
                dataTableImages.Columns.Add(GlobalVariables.IMAGE_BOX_FIELD, typeof(bool));
                foreach (DataRow row in dataTableImages.Rows)
                {
                    string idWorker = row[0].ToString();
                    row[GlobalVariables.IMAGE_FIELD] = GetImageWorker(idWorker);
                    row[GlobalVariables.IMAGE_BOX_FIELD] = true;
                }
                WorkerDetailsTableProperty = dataTableImages;
            });            
        }

        //Function that updates to 1 the Analayzer's flag in the azure database
        private async Task<int> UpdateHandleInAnalayzerConfig()
        {
            string updateQuery = @"UPDATE [" + GlobalVariables.DBO_NAME + "].[" + GlobalVariables.ANALAYZER_CONFIG_TABLE_NAME + "] " +
                "SET " + GlobalVariables.HANDLE_ANALAYZER_FIELD + " = @" + GlobalVariables.HANDLE_ANALAYZER_FIELD + "";
            Dictionary<string, string> fieldNameToValueDict = new Dictionary<string, string>
            {
                { "@" + GlobalVariables.HANDLE_ANALAYZER_FIELD + "", "1" }
            };
            await QueryDatabaseWithDict(updateQuery, fieldNameToValueDict);
            return default;
        }

        //Function that insert worker to azure database and updating the WorkerDetailsTableProperty
        public async Task<bool> InsertWorker(string idWorker, string fullname, string emailAddress, BitmapImage imagePath)
        {
            byte[] imageToByte = ImagePathToByteArray(imagePath);
            await UploadImageToStorage(idWorker, imageToByte);
            DataTable workerDetailsTableTemp = WorkerDetailsTableProperty;
            workerDetailsTableTemp.Rows.Add(idWorker, fullname, emailAddress, imageToByte, true);
            WorkerDetailsTableProperty = workerDetailsTableTemp;
            string insertQuery = @"INSERT INTO [" + GlobalVariables.DBO_NAME + "].[" + GlobalVariables.WORKERS_TABLE_NAME + "] " +
                "VALUES (@" + GlobalVariables.ID_FIELD + ", @" + GlobalVariables.FULL_NAME_FIELD + ", @" + GlobalVariables.EMAIL_ADDRESS_FIELD + ");";
            Dictionary<string, string> fieldNameToValueDict = new Dictionary<string, string>
                {
                    { "@" + GlobalVariables.ID_FIELD + "", idWorker },
                    { "@" + GlobalVariables.FULL_NAME_FIELD + "", fullname },
                    { "@" + GlobalVariables.EMAIL_ADDRESS_FIELD + "", emailAddress }
                };
            await QueryDatabaseWithDict(insertQuery, fieldNameToValueDict);
            return default;
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
        public async Task<bool> UpdateWorkerDetails(string idWorker, string fullname, string emailAddress, BitmapImage imagePath, int indexOfSelectedRow)
        {
            byte[] imageToByte = ImagePathToByteArray(imagePath);
            DataTable workerDetailsTableTemp = WorkerDetailsTableProperty;
            var rowToChange = workerDetailsTableTemp.Rows[indexOfSelectedRow];
            await UploadImageToStorage(idWorker, imageToByte);
            rowToChange[GlobalVariables.ID_FIELD] = idWorker;
            rowToChange[GlobalVariables.FULL_NAME_FIELD] = fullname;
            rowToChange[GlobalVariables.EMAIL_ADDRESS_FIELD] = emailAddress;
            rowToChange[GlobalVariables.IMAGE_FIELD] = imageToByte;
            WorkerDetailsTableProperty = workerDetailsTableTemp;
            string updateQuery = @"UPDATE [" + GlobalVariables.DBO_NAME + "].[" + GlobalVariables.WORKERS_TABLE_NAME + "] " +
                "SET " + GlobalVariables.FULL_NAME_FIELD + " = @" + GlobalVariables.FULL_NAME_FIELD + ", " +
                "" + GlobalVariables.EMAIL_ADDRESS_FIELD + " = @" + GlobalVariables.EMAIL_ADDRESS_FIELD + " " +
                "Where " + GlobalVariables.ID_FIELD + " = @" + GlobalVariables.ID_FIELD + ";";
            Dictionary<string, string> fieldNameToValueDict = new Dictionary<string, string>
                {
                    { "@" + GlobalVariables.ID_FIELD + "", idWorker },
                    { "@" + GlobalVariables.FULL_NAME_FIELD + "", fullname },
                    { "@" + GlobalVariables.EMAIL_ADDRESS_FIELD + "", emailAddress }
                };
            await QueryDatabaseWithDict(updateQuery, fieldNameToValueDict);            
            return true;
        }
        //Function that updates worker's details that has a new id and updates the WorkerDetalisTable
        public async Task<bool> UpdateWorkerDetailsWithNewId(string idWorkerInDataTable, string idWorker, string fullname, string emailAddress, BitmapImage imagePath, int indexOfSelectedRow)
        {
            //It does not exists so updates the worker's details
            byte[] imageToByte = ImagePathToByteArray(imagePath);
            DataTable workerDetailsTableTemp = WorkerDetailsTableProperty;
            var rowToChange = workerDetailsTableTemp.Rows[indexOfSelectedRow];
            await UploadImageToStorage(idWorker, imageToByte);
            rowToChange[GlobalVariables.ID_FIELD] = idWorker;
            rowToChange[GlobalVariables.FULL_NAME_FIELD] = fullname;
            rowToChange[GlobalVariables.EMAIL_ADDRESS_FIELD] = emailAddress;
            rowToChange[GlobalVariables.IMAGE_FIELD] = imageToByte;
            WorkerDetailsTableProperty = workerDetailsTableTemp;
            await UpdateWorkerId(idWorkerInDataTable, idWorker, fullname, emailAddress);            
            return default;
        }

        //Function that updates worker's details in case the worker's id has been changed
        private async Task UpdateWorkerIdToNewId(string newIdWorker, string idWorkerToUpdate, string fullname, string emailAddress)
        {
            string updateQuery = @"UPDATE [" + GlobalVariables.DBO_NAME + "].[" + GlobalVariables.WORKERS_TABLE_NAME + "] " +
                "SET " + GlobalVariables.ID_FIELD + " = @" + GlobalVariables.ID_FIELD + ", " +
                "" + GlobalVariables.FULL_NAME_FIELD + " = @" + GlobalVariables.FULL_NAME_FIELD + ", " +
                "" + GlobalVariables.EMAIL_ADDRESS_FIELD + " = @" + GlobalVariables.EMAIL_ADDRESS_FIELD + " " +
                "Where " + GlobalVariables.ID_FIELD + " = '" + idWorkerToUpdate + "'";
            Dictionary<string, string> fieldNameToValueDict = new Dictionary<string, string>
            {
                { "@" + GlobalVariables.ID_FIELD + "", newIdWorker },
                { "@" + GlobalVariables.FULL_NAME_FIELD + "", fullname },
                { "@" + GlobalVariables.EMAIL_ADDRESS_FIELD + "", emailAddress }
            };
            await QueryDatabaseWithDict(updateQuery, fieldNameToValueDict);
        }

        //Function that resposible for updating worker's details in case the worker's id has been changed,
        //so we need to delete and save all the worker's events, updating worker's id and then insert the events with the new id
        private async Task UpdateWorkerId(string idWorkerInDataTable, string idWorker, string fullname, string emailAddress)
        {
            List<object[]> eventsByIdList = QuerySelectOfMultiRows("select * " +
                "from [" + GlobalVariables.DBO_NAME + "].[" + GlobalVariables.HISTORY_EVENTS_TABLE_NAME + "] " +
                "where " + GlobalVariables.ID_WORKER_FIELD + " = '" + idWorkerInDataTable + "'");
            if (eventsByIdList != null)
            {
                string deleteQuery = @"DELETE FROM [" + GlobalVariables.DBO_NAME + "].[" + GlobalVariables.HISTORY_EVENTS_TABLE_NAME + "] " +
                    "WHERE " + GlobalVariables.ID_WORKER_FIELD + " = '" + idWorkerInDataTable + "';";
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
            string deleteQuery = @"DELETE FROM [" + GlobalVariables.DBO_NAME + "].[" + GlobalVariables.WORKERS_TABLE_NAME + "] " +
                "WHERE " + GlobalVariables.ID_FIELD + " = '" + idWorker + "';";
            await QueryDatabaseWithDict(deleteQuery);
            await DeleteImageFromStorage(idWorker);
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
