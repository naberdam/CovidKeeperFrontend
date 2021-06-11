using CovidKeeperFrontend.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovidKeeperFrontend.ViewModel
{
    public class SearchWorkersVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public WpfDatabase model;
        public SearchWorkersVM(WpfDatabase modelCreated)
        {
            this.model = modelCreated;
            // Notify to view from model.
            model.PropertyChanged += delegate (Object sender, PropertyChangedEventArgs e)
            {
                NotifyPropertyChanged("VM_" + e.PropertyName);
            };
        }

        public void NotifyPropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
        public DataTable VM_SearchWorkerDetailsTableProperty
        {
            get { return model.SearchWorkerDetailsTableProperty; }
            /*set { model.SearchWorkerDetailsTableProperty = null; }*/
        }

        public void SearchById(string idWorker)
        {
            this.model.SearchById(idWorker);
        }
        public void SearchByIdAndEmail(string idWorker, string emailAddress)
        {
            /*this.model.SearchByIdAndEmail(idWorker, emailAddress);*/
        }
        public void SearchByIdAndFullName(string idWorker, string fullName)
        {
            /*this.model.SearchByIdAndFullName(idWorker, fullName);*/
        }
        public void SearchByFullName(string fullName)
        {
            this.model.SearchByFullName(fullName);
        }
        public void SearchByFullNameAndEmail(string fullName, string emailAddress)
        {
            /*this.model.SearchByFullNameAndEmail(fullName, emailAddress);*/
        }
        public void SearchByEmail(string emailAddress)
        {
            this.model.SearchByEmail(emailAddress);
        }
        public void SearchByIdAndFullNameAndEmail(string idWorker, string fullName, string emailAddress)
        {
            //this.model.SearchByIdAndFullNameAndEmail(idWorker, fullName, emailAddress);
        }

    }
}
