using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CovidKeeperFrontend.Model.Database
{
    public sealed class AzureSingleton
    {
        private readonly SqlConnection sqlConnection;
        private static readonly Lazy<AzureSingleton> lazy = new Lazy<AzureSingleton>(() => new AzureSingleton());
        private readonly CloudStorageAccount cloudStorageAccount;
        private const string configFileName = "CovidKeeperFrontend\\Files\\configAzure.json";
        private const string databaseKey = "Database";
        private const string storageKey = "Storage";
        private const string endOfFile = ".jpg";
        private const string containerName = "pictures";

        
        private AzureSingleton()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            path = path.Substring(0, path.Length - 4);
            path += configFileName;
            Dictionary<string, string> configDict = new Dictionary<string, string>();
            using (StreamReader r = new StreamReader(path))
            {
                string json = r.ReadToEnd();
                configDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            }
            //Check if we succeed to read from the file or not
            if (configDict == null || configDict.Count == 0)
            {
                throw new Exception("The file is not exists or it is empty");
            }
            //After we succeed to read from the file we want to check if the databaseKey is exists there
            if (!configDict.ContainsKey(databaseKey))
            {
                throw new Exception("The key " + databaseKey + " is not exists in " + configFileName + " file");
            }
            //After we succeed to read from the file we want to check if the storageKey is exists there
            if (!configDict.ContainsKey(storageKey))
            {
                throw new Exception("The key " + storageKey + " is not exists in " + configFileName + " file");
            }
            string connectionString = configDict[databaseKey];
            sqlConnection = new SqlConnection(connectionString);
            //Connect to the Azure SQL database
            try
            {
                sqlConnection.Open();
            }
            catch (Exception)
            {
                MessageBox.Show("There is a problem with the connection to database.\nPlease try again");
                System.Environment.Exit(0);
            }            
            //Define our CloudStorageAccount
            cloudStorageAccount = CloudStorageAccount.Parse(configDict[storageKey]);
        }
        
        //Instance for implementation of Singleton class
        public static AzureSingleton Instance { get { return lazy.Value; } }

        //Function that return list of object[] according to the given query
        public List<object[]> QuerySelectOfMultiRows(string selectQuery)
        {
            List<object[]> result_list = new List<object[]>();
            using (var command = sqlConnection.CreateCommand())
            {
                command.CommandText = selectQuery;
                using (var reader = command.ExecuteReader())
                {
                    //Loop on the results from SQL
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
        //Function that return object[] which represent one row from the given query
        public object[] QuerySelectOfOneRow(string selectQuery)
        {
            object[] lineInformationFromSQL;
            using (var command = sqlConnection.CreateCommand())
            {
                command.CommandText = selectQuery;
                using (var reader = command.ExecuteReader())
                {
                    //Get the result from the SQL
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
        //Function that checks if generic T is null or not
        private T CheckIfNull<T>(T valueToCheck)
        {
            if (valueToCheck == null)
            {
                return default;
            }
            return valueToCheck;
        }

        //Function that returns the cloudBlobContainer for upload or download images from storage
        private CloudBlobContainer GetCloudBlobContainer()
        {
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(containerName);
            cloudBlobContainer.CreateIfNotExists();
            //It is a Blob Storage
            cloudBlobContainer.SetPermissions(new BlobContainerPermissions() { PublicAccess = BlobContainerPublicAccessType.Blob });
            return cloudBlobContainer;
        }

        //Function that upload image to the azure storage
        public async Task UploadImageToStorage(string idWorker, byte[] imageToByte)
        {
            CloudBlobContainer cloudBlobContainer = GetCloudBlobContainer();
            CloudBlockBlob blockBlob = cloudBlobContainer.GetBlockBlobReference(idWorker + endOfFile);
            using (var stream = new MemoryStream(imageToByte, writable: false))
            {
                //Upload async
                await blockBlob.UploadFromStreamAsync(stream);
            }
        }
        //Function that deletes image from storage according to given id
        public async Task DeleteImageFromStorage(string idWorker)
        {
            CloudBlobContainer cloudBlobContainer = GetCloudBlobContainer();
            CloudBlockBlob blockBlob = cloudBlobContainer.GetBlockBlobReference(idWorker + endOfFile);
            await blockBlob.DeleteIfExistsAsync();
        }
        //Function that returns worker's image according to the given idWorker
        public byte[] GetImageWorker(string idWorker)
        {
            CloudBlobContainer cloudBlobContainer = GetCloudBlobContainer();
            CloudBlockBlob blockBlob = cloudBlobContainer.GetBlockBlobReference(idWorker + endOfFile);
            using (var memoryStream = new MemoryStream())
            {
                blockBlob.DownloadToStream(memoryStream);
                return memoryStream.ToArray();
            }
        }
        //Function that responsible to the queries like insert, upload and delete in azure SQL.
        //fieldNameToValueDict is a dictionary with the field names of the table that we want to change
        //with their values.
        public async Task QueryDatabaseWithDict(string updateQuery, Dictionary<string,string> fieldNameToValueDict)
        {
            using (var command = sqlConnection.CreateCommand())
            {
                command.CommandText = updateQuery;
                if (fieldNameToValueDict != null)
                {
                    foreach (var item in fieldNameToValueDict)
                    {
                        command.Parameters.AddWithValue(item.Key, item.Value);
                    }
                }
                                
                await command.ExecuteNonQueryAsync();
            }
        }
        //Function that returns a datatable according to the given query
        public DataTable GetDataTableByQuery(string selectQuery, string tableName)
        {
            SqlCommand command = new SqlCommand
            {
                CommandText = selectQuery,
                Connection = sqlConnection
            };
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
            DataTable dataTable = new DataTable(tableName);
            //Fill the datatable with the values from the query result
            sqlDataAdapter.Fill(dataTable);
            return dataTable;
        }
        //Function that update the time break between mails while using the given minutes
        public async Task UpdateTimeBreakForMails(int minutesBreakToChange)
        {
            using (var command = sqlConnection.CreateCommand())
            {
                string updateQuery = @"UPDATE [dbo].[Manager_Config] SET Minutes_between_mails = @Minutes_between_mails, Handle = @Handle";
                command.CommandText = updateQuery;
                command.Parameters.AddWithValue("@Minutes_between_mails", minutesBreakToChange);
                command.Parameters.AddWithValue("@Handle", 1);
                await command.ExecuteNonQueryAsync();
            }
        }

        //Function that insert events by id according to the given id in case that the client wanted to edit the worker's id
        public async Task InsertEventsListById(List<object[]> eventsList, string idWorker)
        {
            string insertStmt = "INSERT INTO [dbo].[History_Events] VALUES(@Id_worker, @Time_of_event)";
            using (var cmd = sqlConnection.CreateCommand())
            {
                cmd.CommandText = insertStmt;
                cmd.Parameters.Add("@Id_worker", SqlDbType.VarChar).Value = idWorker;
                cmd.Parameters.Add("@Time_of_event", SqlDbType.DateTime);
                //Iterate over all Time_of_event's and execute the INSERT statement for each of them
                foreach (var item in eventsList)
                {
                    cmd.Parameters["@Time_of_event"].Value = Convert.ToDateTime(item[1]);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
