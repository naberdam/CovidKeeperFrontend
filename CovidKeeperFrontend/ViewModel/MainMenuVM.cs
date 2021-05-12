using CovidKeeperFrontend.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovidKeeperFrontend.ViewModel
{
    public class MainMenuVM
    {
        public WpfDatabase model;
        public MainMenuVM(WpfDatabase modelCreated)
        {
            this.model = modelCreated;
        }
        public int VM_IndexOfMenuListProperty
        {
            set { model.IndexOfMenuListProperty = value; }
        }

    }
}
