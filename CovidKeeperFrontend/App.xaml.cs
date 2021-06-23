using CovidKeeperFrontend.Model;
using CovidKeeperFrontend.Model.Database;
using CovidKeeperFrontend.ViewModel;
using CovidKeeperFrontend.Views;
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
        public ManageWorkersVM ManageWorkersViewModel { get; internal set; }
        public MainMenuVM MainMenuViewModel { get; internal set; }
        public StatisticalDataVM StatisticalDataViewModel { get; internal set; }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            HomeViewModel = new HomeVM(Home);
            ManageWorkersViewModel = new ManageWorkersVM(ManageWorkers);
            MainMenuViewModel = new MainMenuVM(MainMenu);
            StatisticalDataViewModel = new StatisticalDataVM(StatisticalData);
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //initialize the splash screen and set it as the application main window
            var splashScreen = new SplashScreenWindow();
            this.MainWindow = splashScreen;
            splashScreen.Show();

            //in order to ensure the UI stays responsive, we need to
            //do the work on a different thread
            Task.Factory.StartNew(() =>
            {
                //we need to do the work in batches so that we can report progress
                for (int i = 1; i <= 100; i++)
                {
                    //simulate a part of work being done
                    System.Threading.Thread.Sleep(30);

                    //because we're not on the UI thread, we need to use the Dispatcher
                    //associated with the splash screen to update the progress bar
                    splashScreen.Dispatcher.Invoke(() => splashScreen.Progress = i);
                }

                //once we're done we need to use the Dispatcher
                //to create and show the main window
                this.Dispatcher.Invoke(() =>
                {
                    //initialize the main window, set it as the application main window
                    //and close the splash screen
                    var mainWindow = new MainMenu();
                    this.MainWindow = mainWindow;
                    
                    splashScreen.Close();
                    mainWindow.Show();
                });
            });
        }
    }
}
