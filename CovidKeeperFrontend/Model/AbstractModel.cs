using CovidKeeperFrontend.Model.Database;
using Microsoft.Azure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovidKeeperFrontend.Model
{
    public abstract class AbstractModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected AzureSingleton azureSingleton;

        public AbstractModel()
        {
            azureSingleton = AzureSingleton.Instance;
        }
        public void NotifyPropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
        public abstract Task<int> RefreshDataAsync();
        public List<object[]> QuerySelectOfMultiRows(string selectQuery)
        {
            return azureSingleton.QuerySelectOfMultiRows(selectQuery);
        }
        public object[] QuerySelectOfOneRow(string selectQuery)
        {
            return azureSingleton.QuerySelectOfOneRow(selectQuery);
        }
        public byte[] GetImageWorker(string idWorker)
        {
            return azureSingleton.GetImageWorker(idWorker);
        }
        public async Task UploadImageToStorage(string idWorker, byte[] imageToByte)
        {
            await azureSingleton.UploadImageToStorage(idWorker, imageToByte);
        }
        public async Task UpdateWithOneParameter(string updateQuery, string nameFieldToUpdate, string valueToUpdate)
        {
            await azureSingleton.UpdateWithOneParameter(updateQuery, nameFieldToUpdate, valueToUpdate);
        }
        public async Task QueryDatabaseWithDict(string updateQuery, Dictionary<string, string> fieldNameToValueDict=default)
        {
            await azureSingleton.QueryDatabaseWithDict(updateQuery, fieldNameToValueDict);
        }
        public DataTable GetDataTableByQuery(string selectQuery, string tableName)
        {
            return azureSingleton.GetDataTableByQuery(selectQuery, tableName);
        }
    }
}
