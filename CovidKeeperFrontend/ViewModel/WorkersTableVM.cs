using CovidKeeperFrontend.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace CovidKeeperFrontend.ViewModel
{
    public class WorkersTableVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public WpfDatabase model;

        public WorkersTableVM(WpfDatabase modelCreated)
        {
            this.model = modelCreated;
            // Notify to view from model.
            model.PropertyChanged += delegate (Object sender, PropertyChangedEventArgs e)
            {
                NotifyPropertyChanged("VM_" + e.PropertyName);
            };
        }
        public DataTable VM_WorkerDetailsTableProperty
        {
            get { return model.WorkerDetailsTableProperty; }
        }

        public void NotifyPropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
        public void InsertWorker(string idWorker, string fullname, string emailAddress, BitmapImage imagePath)
        {
            this.model.InsertWorker(idWorker, fullname, emailAddress, imagePath);
        }
        public void UpdateWorkerDetails(string idWorker, string fullname, string emailAddress, BitmapImage imagePath, int indexOfSelectedRow)
        {
            this.model.UpdateWorkerDetails(idWorker, fullname, emailAddress, imagePath, indexOfSelectedRow);
        }
        public void DeleteWorker(string idWorker, int indexOfSelectedRow)
        {
            this.model.DeleteWorker(idWorker, indexOfSelectedRow);
        }
    }
}
