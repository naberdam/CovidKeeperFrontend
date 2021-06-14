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
    public class ManageWorkersModel : AbstractModel
    {
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
                    SearchOrWorkersTableProperty = false;
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
                    CountWorkersInWorkersDetailsTableProperty = value.Rows.Count.ToString();
                    SearchOrWorkersTableProperty = true;
                    NotifyPropertyChanged("WorkerDetailsTableProperty");
                }
            }
        }
        private bool searchOrWorkersTable = false;

        public bool SearchOrWorkersTableProperty
        {
            get { return searchOrWorkersTable; }
            set { searchOrWorkersTable = value; }
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
        public NotifyTaskCompletion<int> RefreshData { get; private set; }
        public ManageWorkersModel()
        {
            RefreshData = new NotifyTaskCompletion<int>(RefreshDataAsync());
        }

        public override async Task<int> RefreshDataAsync()
        {
            await GetWorkersDetailsTemp();
            return 1;
        }

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
        public void SearchById(string idWorker)
        {
            SearchWorkerDetailsTableProperty = SearchTableByQuery("Id like '%" + idWorker + "%'");
        }
        public void SearchByFullName(string fullName)
        {
            SearchWorkerDetailsTableProperty = SearchTableByQuery("FullName like '%" + fullName + "%'");
        }
        public void SearchByEmail(string emailAddress)
        {
            SearchWorkerDetailsTableProperty = SearchTableByQuery("Email_address like '%" + emailAddress + "%'");
        }
        public async Task GetWorkersDetailsTemp()
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
            string updateQuery = @"UPDATE [dbo].[Workers] SET FullName = @FullName, Email_address = @Email_address Where Id = @Id;";
            Dictionary<string, string> fieldNameToValueDict = new Dictionary<string, string>
            {
                { "@Id", idWorker },
                { "@FullName", fullname },
                { "@Email_address", emailAddress }
            };
            await QueryDatabaseWithDict(updateQuery, fieldNameToValueDict);
            byte[] imageToByte = ImagePathToByteArray(imagePath);
            DataTable workerDetailsTableTemp = WorkerDetailsTableProperty;
            var rowToChange = workerDetailsTableTemp.Rows[indexOfSelectedRow];
            await UploadImageToStorage(idWorker, imageToByte);
            rowToChange["Id"] = idWorker;
            rowToChange["Fullname"] = fullname;
            rowToChange["Email_address"] = emailAddress;
            rowToChange["Image"] = imageToByte;
            WorkerDetailsTableProperty = workerDetailsTableTemp;
            return default;
        }

        public async Task DeleteWorker(string idWorker, int indexOfSelectedRow)
        {
            string deleteQuery = @"DELETE FROM [dbo].[Workers] WHERE Id = " + idWorker + ";";
            await QueryDatabaseWithDict(deleteQuery);
            DataTable workerDetailsTableTemp = WorkerDetailsTableProperty;
            var rowToChange = workerDetailsTableTemp.Rows[indexOfSelectedRow];
            rowToChange.Delete();
            WorkerDetailsTableProperty = workerDetailsTableTemp;
        }
        public void GetWorkersDetailsAfterRefresh()
        {
            SearchOrWorkersTableProperty = false;
            NotifyPropertyChanged("WorkerDetailsTableProperty");
        }
    }
}
