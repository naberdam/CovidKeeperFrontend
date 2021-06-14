using CovidKeeperFrontend.Model;
using CovidKeeperFrontend.Model.Database;
using CovidKeeperFrontend.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CovidKeeperFrontend
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /*public WpfDatabase WpfDatabaseObject = new WpfDatabase();*/
        public HomeModel Home = new HomeModel();
        public MainMenuModel MainMenu = new MainMenuModel();
        public ManageWorkersModel ManageWorkers = new ManageWorkersModel();
        public StatisticalDataModel StatisticalData = new StatisticalDataModel();
        public HomeVM HomeViewModel { get; internal set; }
        public WorkersTableVM WorkersTableViewModel { get; internal set; }
        public MainMenuVM MainMenuViewModel { get; internal set; }
        public StatisticalDataVM StatisticalDataViewModel { get; internal set; }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            /*HomeViewModel = new HomeVM(WpfDatabaseObject);
            WorkersTableViewModel = new WorkersTableVM(WpfDatabaseObject);
            MainMenuViewModel = new MainMenuVM(WpfDatabaseObject);
            StatisticalDataViewModel = new StatisticalDataVM(WpfDatabaseObject);*/
            HomeViewModel = new HomeVM(Home);
            WorkersTableViewModel = new WorkersTableVM(ManageWorkers);
            MainMenuViewModel = new MainMenuVM(MainMenu);
            StatisticalDataViewModel = new StatisticalDataVM(StatisticalData);
        }
    }
}
