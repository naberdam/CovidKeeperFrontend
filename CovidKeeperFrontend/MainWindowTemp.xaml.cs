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
        private readonly HomeUserControl homeUserControl = new HomeUserControl();
        private readonly StatisticalDataUserControl statisticalDataUserControl = new StatisticalDataUserControl();
        private readonly ManageWorkersUserControl manageWorkersUserControl = new ManageWorkersUserControl();
        public MainWindowTemp()
        {
            InitializeComponent();
            DataContext = (Application.Current as App).MainMenuViewModel;
            homeUserControl.DataContext = (Application.Current as App).HomeViewModel;
            statisticalDataUserControl.DataContext = (Application.Current as App).StatisticalDataViewModel;
            manageWorkersUserControl.DataContext = (Application.Current as App).ManageWorkersViewModel;
            ListViewMenu.SelectedIndex = 0;
            manageWorkersUserControl.SetMainWindow(this);
            statisticalDataUserControl.SetMainWindow(this);
            UpdateBreakTimeForSendMailButton.IsEnabled = false;
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
                    (Application.Current as App).HomeViewModel.RefreshData();
                    GridPrincipal.Children.Clear();                    
                    GridPrincipal.Children.Add(homeUserControl);                    
                    break;
                case 1:
                    (Application.Current as App).ManageWorkersViewModel.RefreshData();
                    manageWorkersUserControl.ClearFields();
                    GridPrincipal.Children.Clear();
                    GridPrincipal.Children.Add(manageWorkersUserControl);
                    break;
                case 2:
                    (Application.Current as App).StatisticalDataViewModel.RefreshData();
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
            if (BreakTimeForSendMailText.Text != "" && isNumeric && minutes >= 0)
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

        private void BreakTimeForSendMailText_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if ((Application.Current as App).MainMenuViewModel.MinutesBreakForMailsProperty == null)
            {
                UpdateBreakTimeForSendMailButton.IsEnabled = false;
            }
            else if ((Application.Current as App).MainMenuViewModel.MinutesBreakForMailsProperty.Length != textBox.Text.Length)
            {
                UpdateBreakTimeForSendMailButton.IsEnabled = false;
            }
            else
            {
                UpdateBreakTimeForSendMailButton.IsEnabled = true;
            }
        }
    }
}
