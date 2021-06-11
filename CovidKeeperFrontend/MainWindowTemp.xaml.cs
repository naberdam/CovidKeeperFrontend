using CovidKeeperFrontend.ViewModel;
using CovidKeeperFrontend.Views;
using MaterialDesignThemes.Wpf;
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
        StatisticalDataUserControl statisticalDataUserControl = new StatisticalDataUserControl();
        ManageWorkersUserControl manageWorkersUserControl = new ManageWorkersUserControl();
        public MainWindowTemp()
        {
            InitializeComponent();
            DataContext = (Application.Current as App).MainMenuViewModel;
            homeUserControl.DataContext = (Application.Current as App).HomeViewModel;
            statisticalDataUserControl.DataContext = (Application.Current as App).StatisticalDataViewModel;
            manageWorkersUserControl.DataContext = (Application.Current as App).WorkersTableViewModel;
            ListViewMenu.SelectedIndex = 0;
            manageWorkersUserControl.SetMainWindow(this);
            statisticalDataUserControl.SetMainWindow(this);
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
                    manageWorkersUserControl.ClearFields();
                    GridPrincipal.Children.Clear();
                    GridPrincipal.Children.Add(manageWorkersUserControl);
                    break;
                case 2:
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
            GridCursor.Margin = new Thickness(0, (190 + (60 * index)), 0, 0);
        }

        private void UpdateBreakTimeForSendMailButton_Click(object sender, RoutedEventArgs e)
        {
            var isNumeric = int.TryParse(BreakTimeForSendMailText.Text, out int minutes);
            if (BreakTimeForSendMailText.Text != "" && isNumeric)
            {
                (Application.Current as App).MainMenuViewModel.VM_MinutesBreakForMailsProperty = minutes;
                this.IsEnabled = true;
                DialogHost.CloseDialogCommand.Execute(null, null);
            }
            else if (!isNumeric)
            {
                MessageBox.Show("You need to insert minutes only with numbers (and not letters)");
            }
        }

        private void CancelBreakTimeForSendMailButton_Click(object sender, RoutedEventArgs e)
        {
            DialogHost.CloseDialogCommand.Execute(null, null);
            this.IsEnabled = true;
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
        }
    }
}
