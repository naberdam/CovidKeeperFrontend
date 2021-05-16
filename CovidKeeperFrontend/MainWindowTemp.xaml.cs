using CovidKeeperFrontend.ViewModel;
using CovidKeeperFrontend.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CovidKeeperFrontend
{
    /// <summary>
    /// Interaction logic for MainWindowTemp.xaml
    /// </summary>
    public partial class MainWindowTemp : Window
    {
        HomeUserControl homeUserControl = new HomeUserControl();
        WorkersTableUserControl workersTableUserControl = new WorkersTableUserControl();
        SearchWorkersUserControl searchWorkersUserControl = new SearchWorkersUserControl();
        StatisticalDataUserControl statisticalDataUserControl = new StatisticalDataUserControl();
        public MainWindowTemp()
        {
            InitializeComponent();
            DataContext = (Application.Current as App).MainMenuViewModel;
            homeUserControl.DataContext = (Application.Current as App).HomeViewModel;
            workersTableUserControl.DataContext = (Application.Current as App).WorkersTableViewModel;
            searchWorkersUserControl.DataContext = (Application.Current as App).SearchWorkersViewModel;
            statisticalDataUserControl.DataContext = (Application.Current as App).StatisticalDataViewModel;
            ListViewMenu.SelectedIndex = 0;
        }

        private void ButtonFechar_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ListViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = ListViewMenu.SelectedIndex;
            MoveCursorMenu(index);

            switch (index)
            {
                case 0:
                    GridPrincipal.Children.Clear();                    
                    GridPrincipal.Children.Add(homeUserControl);                    
                    break;
                case 1:
                    workersTableUserControl.ClearFields();
                    GridPrincipal.Children.Clear();
                    GridPrincipal.Children.Add(workersTableUserControl);
                    break;
                case 2:
                    searchWorkersUserControl.ClearFields();
                    GridPrincipal.Children.Clear();
                    GridPrincipal.Children.Add(searchWorkersUserControl);
                    break;
                case 3:
                    statisticalDataUserControl.ClearFields();
                    GridPrincipal.Children.Clear();
                    GridPrincipal.Children.Add(statisticalDataUserControl);
                    break;
                default:
                    break;
            }
        }

        private void MoveCursorMenu(int index)
        {
            TransitioningContentSlide.OnApplyTemplate();
            GridCursor.Margin = new Thickness(0, (100 + (60 * index)), 0, 0);
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UpdateBreakTimeForSendMailButton_Click(object sender, RoutedEventArgs e)
        {
            var isNumeric = int.TryParse(BreakTimeForSendMailText.Text, out int minutes);
            if (BreakTimeForSendMailText.Text != "" && isNumeric)
            {
                (Application.Current as App).MainMenuViewModel.VM_MinutesBreakForMailsProperty = minutes;
            }
        }

        private void CancelBreakTimeForSendMailButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
