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
            //Get the singleton instance of AzureSingleton
            azureSingleton = AzureSingleton.Instance;
        }
        public void NotifyPropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
        //Abstract function that updates the information in each view
        public abstract Task<int> RefreshDataAsync();
        //Function that returns list of object[] that represent the results from the given query
        public List<object[]> QuerySelectOfMultiRows(string selectQuery)
        {
            return azureSingleton.QuerySelectOfMultiRows(selectQuery);
        }
        //Function that returns object[] that represent the result from the given query
        public object[] QuerySelectOfOneRow(string selectQuery)
        {
            return azureSingleton.QuerySelectOfOneRow(selectQuery);
        }
        //Function that returns worker's image by the idWorker
        public byte[] GetImageWorker(string idWorker)
        {
            return azureSingleton.GetImageWorker(idWorker);
        }
        //Function that uploads image to blob storage
        public async Task UploadImageToStorage(string idWorker, byte[] imageToByte)
        {
            await azureSingleton.UploadImageToStorage(idWorker, imageToByte);
        }
        public async Task DeleteImageFromStorage(string idWorker)
        {
            await azureSingleton.DeleteImageFromStorage(idWorker);
        }
        //Function that responsible to the queries like insert, upload and delete in azure SQL.
        //fieldNameToValueDict is a dictionary with the field names of the table that we want to change
        //with their values. 
        public async Task QueryDatabaseWithDict(string updateQuery, Dictionary<string, string> fieldNameToValueDict=default)
        {
            await azureSingleton.QueryDatabaseWithDict(updateQuery, fieldNameToValueDict);
        }
        //Function that returns a datatable according to the given query
        public DataTable GetDataTableByQuery(string selectQuery, string tableName)
        {
            return azureSingleton.GetDataTableByQuery(selectQuery, tableName);
        }
        //Function that insert events by id according to the given id in case that the client wanted to edit the worker's id
        public async Task InsertEventsListById(List<object[]> eventsList, string idWorker)
        {
            await azureSingleton.InsertEventsListById(eventsList, idWorker);
        }
        //Function that check if the given datatables are the same
        public static bool AreTablesTheSame(DataTable tbl1, DataTable tbl2)
        {
            if (tbl1 == default && tbl2 == default)
            {
                return true;
            }
            if (tbl1 == default || tbl2 == default)
            {
                return false;
            }
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
    }
}
