using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovidKeeperFrontend.Model.Database
{
    public sealed class AzureSingleton
    {
        private readonly string connectionString;
        private readonly SqlConnection sqlConnection;
        private static readonly Lazy<AzureSingleton> lazy = new Lazy<AzureSingleton>(() => new AzureSingleton());
        private readonly CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;" +
                "AccountName=faceimages2;" +
                "AccountKey=vlaKfbwxn8eU1kZGo3KjuFIsgQ0BGot1MRCvs6x0mB923Yx2FOXv4XQ82Hgi/l4iKb4iM/DSNcAeezmYYxxFxw==;" +
                "EndpointSuffix=core.windows.net");
        private AzureSingleton()
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
        }
        

        public static AzureSingleton Instance { get { return lazy.Value; } }

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
        private CloudBlobContainer GetCloudBlobContainer()
        {
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("pictures");
            cloudBlobContainer.CreateIfNotExists();
            cloudBlobContainer.SetPermissions(new BlobContainerPermissions() { PublicAccess = BlobContainerPublicAccessType.Blob });
            return cloudBlobContainer;
        }

        public async Task UploadImageToStorage(string idWorker, byte[] imageToByte)
        {
            CloudBlobContainer cloudBlobContainer = GetCloudBlobContainer();
            CloudBlockBlob blockBlob = cloudBlobContainer.GetBlockBlobReference(idWorker + ".jpg");
            using (var stream = new MemoryStream(imageToByte, writable: false))
            {
                await blockBlob.UploadFromStreamAsync(stream);
            }
        }
        public byte[] GetImageWorker(string idWorker)
        {
            CloudBlobContainer cloudBlobContainer = GetCloudBlobContainer();
            CloudBlockBlob blockBlob = cloudBlobContainer.GetBlockBlobReference(idWorker + ".jpg");
            using (var memoryStream = new MemoryStream())
            {
                blockBlob.DownloadToStream(memoryStream);
                return memoryStream.ToArray();
            }
        }
        public async Task UpdateWithOneParameter(string updateQuery, string nameFieldToUpdate, string valueToUpdate)
        {
            using (var command = sqlConnection.CreateCommand())
            {
                command.CommandText = updateQuery;
                command.Parameters.AddWithValue(nameFieldToUpdate, valueToUpdate);
                await command.ExecuteNonQueryAsync();
            }
        }
        public async Task QueryDatabaseWithDict(string updateQuery, Dictionary<string,string> fieldNameToValueDict)
        {
            using (var command = sqlConnection.CreateCommand())
            {
                command.CommandText = updateQuery;
                foreach(var item in fieldNameToValueDict)
                {
                    command.Parameters.AddWithValue(item.Key, item.Value);
                }                
                await command.ExecuteNonQueryAsync();
            }
        }
        public async Task DeleteQuery(string deleteQuery)
        {
            using (var command = sqlConnection.CreateCommand())
            {
                command.CommandText = deleteQuery;
                await command.ExecuteNonQueryAsync();
            }
        }
        public DataTable GetDataTableByQuery(string selectQuery, string tableName)
        {
            SqlCommand command = new SqlCommand
            {
                CommandText = selectQuery,
                Connection = sqlConnection
            };
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
            DataTable dataTable = new DataTable(tableName);
            sqlDataAdapter.Fill(dataTable);
            return dataTable;
        }
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
    }
}
