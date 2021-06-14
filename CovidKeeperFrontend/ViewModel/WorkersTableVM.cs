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
        /*public WpfDatabase model;

        public WorkersTableVM(WpfDatabase modelCreated)
        {
            this.model = modelCreated;
            // Notify to view from model.
            model.PropertyChanged += delegate (Object sender, PropertyChangedEventArgs e)
            {
                NotifyPropertyChanged("VM_" + e.PropertyName);
            };
        }*/
        public ManageWorkersModel model;

        public WorkersTableVM(ManageWorkersModel modelCreated)
        {
            this.model = modelCreated;
            // Notify to view from model.
            model.PropertyChanged += delegate (Object sender, PropertyChangedEventArgs e)
            {
                NotifyPropertyChanged("VM_" + e.PropertyName);
            };
        }
        public string VM_CountWorkersInWorkersDetailsTableProperty 
        { 
            get { return model.CountWorkersInWorkersDetailsTableProperty; }
        }
        public DataTable VM_WorkerDetailsTableProperty
        {
            get 
            {
                if (model.SearchOrWorkersTableProperty)
                {
                    return model.SearchWorkerDetailsTableProperty;
                }
                return model.WorkerDetailsTableProperty; 
            }
        }
        private string idWorkerRule;

        public string IdWorkerRuleProperty
        {
            get { return idWorkerRule; }
            set
            {
                idWorkerRule = value;
                NotifyPropertyChanged("IdWorkerRuleProperty");
            }
        }
        private string fullNameRule;

        public string FullNameRuleProperty
        {
            get { return fullNameRule; }
            set
            {
                fullNameRule = value;
                NotifyPropertyChanged("FullNameRuleProperty");
            }
        }

        private string emailAddressRule;

        public string EmailAddressRuleProperty
        {
            get { return emailAddressRule; }
            set
            {
                emailAddressRule = value;
                NotifyPropertyChanged("EmailAddressRuleProperty");
            }
        }


        public void NotifyPropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
        public async Task InsertWorker(string idWorker, string fullname, string emailAddress, BitmapImage imagePath)
        {
            await this.model.InsertWorker(idWorker, fullname, emailAddress, imagePath);
        }
        public NotifyTaskCompletion<int> UpdateWorkerDetailsAsync { get; private set; }

        

        public void UpdateWorkerDetails(string idWorker, string fullname, string emailAddress, BitmapImage imagePath, int indexOfSelectedRow)
        {
            UpdateWorkerDetailsAsync = new NotifyTaskCompletion<int>(this.model.UpdateWorkerDetails(idWorker, fullname, emailAddress, imagePath, indexOfSelectedRow));
        }
        public async Task DeleteWorker(string idWorker, int indexOfSelectedRow)
        {
            await this.model.DeleteWorker(idWorker, indexOfSelectedRow);
        }
        public void SearchByEmail(string emailAddress)
        {
            this.model.SearchByEmail(emailAddress);
        }
        public void SearchByFullName(string fullName)
        {
            this.model.SearchByFullName(fullName);
        }
        public void SearchById(string idWorker)
        {
            this.model.SearchById(idWorker);
        }
        public void GetWorkersDetailsAfterRefresh()
        {
            this.model.GetWorkersDetailsAfterRefresh();
        }
        public NotifyTaskCompletion<int> RefreshDataAsync { get; private set; }
        public void RefreshData()
        {
            RefreshDataAsync = new NotifyTaskCompletion<int>(this.model.RefreshDataAsync());
        }
    }
}
