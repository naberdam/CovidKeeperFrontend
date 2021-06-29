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
    public class ManageWorkersVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ManageWorkersModel model;

        public ManageWorkersVM(ManageWorkersModel modelCreated)
        {
            this.model = modelCreated;
            // Notify to view from model.
            model.PropertyChanged += delegate (Object sender, PropertyChangedEventArgs e)
            {
                NotifyPropertyChanged("VM_" + e.PropertyName);
            };
        }
        //Property of binding CountWorkersInWorkersDetailsTableProperty to view
        public string VM_CountWorkersInWorkersDetailsTableProperty 
        { 
            get { return model.CountWorkersInWorkersDetailsTableProperty; }
        }
        //Property of binding SearchWorkerDetailsTableProperty or WorkerDetailsTableProperty to view
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
        //Property of binding IdWorkerRuleProperty to view for checking the rules
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
        //Property of binding FullNameRuleProperty to view for checking the rules
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
        //Property of binding EmailAddressRuleProperty to view for checking the rules
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
        //Property of binding IdWorkerUpdateRuleProperty to view for checking the rules
        private string idWorkerUpdateRule;
        public string IdWorkerUpdateRuleProperty
        {
            get { return idWorkerUpdateRule; }
            set
            {
                idWorkerUpdateRule = value;
                NotifyPropertyChanged("IdWorkerUpdateRuleProperty");
            }
        }
        //Property of binding FullNameUpdateRuleProperty to view for checking the rules
        private string fullNameUpdateRule;
        public string FullNameUpdateRuleProperty
        {
            get { return fullNameUpdateRule; }
            set
            {
                fullNameUpdateRule = value;
                NotifyPropertyChanged("FullNameUpdateRuleProperty");
            }
        }
        //Property of binding EmailAddressUpdateRuleProperty to view for checking the rules
        private string emailAddressUpdateRule;
        public string EmailAddressUpdateRuleProperty
        {
            get { return emailAddressUpdateRule; }
            set
            {
                emailAddressUpdateRule = value;
                NotifyPropertyChanged("EmailAddressUpdateRuleProperty");
            }
        }

        public void NotifyPropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
        public NotifyTaskCompletion<bool> InsertWorkerAsync { get; private set; }
        //Function that responsible on insert worker
        public void InsertWorker(string idWorker, string fullname, string emailAddress, BitmapImage imagePath)
        {
            InsertWorkerAsync = new NotifyTaskCompletion<bool>(this.model.InsertWorker(idWorker, fullname, emailAddress, imagePath));
        }
        public NotifyTaskCompletion<bool> UpdateWorkerDetailsAsync { get; private set; }
        //Function that responsible on updating worker details
        public void UpdateWorkerDetails(string idWorker, string fullname, string emailAddress, BitmapImage imagePath, int indexOfSelectedRow)
        {
            UpdateWorkerDetailsAsync = new NotifyTaskCompletion<bool>(this.model.UpdateWorkerDetails(idWorker, fullname, emailAddress, imagePath, indexOfSelectedRow));
        }
        //Function that responsible on updating worker details while there is new id
        public void UpdateWorkerDetailsWithNewId(string idWorkerInDataTable, string idWorker, string fullname, string emailAddress, BitmapImage imagePath, int indexOfSelectedRow)
        {
            UpdateWorkerDetailsAsync = new NotifyTaskCompletion<bool>(this.model.UpdateWorkerDetailsWithNewId(idWorkerInDataTable, idWorker, fullname, emailAddress, imagePath, indexOfSelectedRow));
        }
        //Function that responsible on deleting worker
        public async Task DeleteWorker(string idWorker, int indexOfSelectedRow)
        {
            await this.model.DeleteWorker(idWorker, indexOfSelectedRow);
        }
        //Function that responsible on search worker by email address
        public void SearchByEmail(string emailAddress)
        {
            this.model.SearchByEmail(emailAddress);
        }
        //Function that responsible on search worker by fullName 
        public void SearchByFullName(string fullName)
        {
            this.model.SearchByFullName(fullName);
        }
        //Function that responsible on search worker by id 
        public void SearchById(string idWorker)
        {
            this.model.SearchById(idWorker);
        }
        //Function that responsible on getting WorkersDetailsTable after click on refresh
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
