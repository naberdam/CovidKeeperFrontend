using CovidKeeperFrontend.HelperClasses;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static CovidKeeperFrontend.Views.StatisticalDataUserControl;

namespace CovidKeeperFrontend.Views
{
    /// <summary>
    /// Interaction logic for StatisticalDataUserControl.xaml
    /// </summary>
    public partial class StatisticalDataUserControl : UserControl
    {
        //Variable that represent the last selected row by client
        DataGridRow gridRowSelected = default;
        //Variable that representing the MainWindow
        MainMenu mainWindow = default;
        //Variable that represents the client's choise which grpah he client wants to see
        StatisticsOptionListEnum statisticsOptionListEnum = StatisticsOptionListEnum.Nothing;
        //Variable that representing the last detailsBtn that the client chose
        Button detailsBtn = default;
        
        //Enum that represents the listview with the names of the graphs
        public enum StatisticsOptionListEnum
        {
            AVG_PER_WEEK = 0,
            AVG_PER_MONTH = 1,
            AVG_PER_WEEKDAY = 2,
            TOTAL_EVENTS = 3,
            Nothing = 4
        }
        public StatisticalDataUserControl()
        {
            InitializeComponent();            
        }

        //Function that reset the values 
        public void ClearFields()
        {
            List<StatisticsOptionListEnum> buttonContentList = new List<StatisticsOptionListEnum>() { StatisticsOptionListEnum.AVG_PER_WEEK, 
                StatisticsOptionListEnum.AVG_PER_MONTH, StatisticsOptionListEnum.AVG_PER_WEEKDAY, StatisticsOptionListEnum.TOTAL_EVENTS };
            List<MyButton> buttons = new List<MyButton>();
            int buttonId = 1;
            //Set the names of the listview
            foreach (var content in buttonContentList)
            {
                buttons.Add(new MyButton
                { 
                    ButtonContent = content.ToString().Replace("_", " "), 
                    ButtonID = buttonId.ToString()
                });
                buttonId++;
            }
            StatisticButtonList.ItemsSource = buttons;
            if (gridRowSelected != default)
            {
                gridRowSelected.DetailsVisibility = Visibility.Collapsed;
            }
            AmountEventPerWeekTable.SelectedIndex = -1;
            ColumnGraph.Visibility = Visibility.Visible;
            PieGraph.Visibility = Visibility.Hidden;
            ScrollOfAmountEventPerWeekTable.Visibility = Visibility.Hidden;
            statisticsOptionListEnum = StatisticsOptionListEnum.Nothing;
            (Application.Current as App).StatisticalDataViewModel.GraphInRangeOrNotProperty = false;
            detailsBtn = default;
            gridRowSelected = default;
        }

        private void AmountEventPerWeekTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            DataRowView rowSelected = gd.SelectedItem as DataRowView;
            //If the client choose a row in total events
            if (rowSelected != null)
            {
                (Application.Current as App).StatisticalDataViewModel.VM_IdWorkerForLineGraphProperty = rowSelected[GlobalVariables.ID_WORKER_FIELD].ToString();
            }            
        }

        //Function that represents the client's choose
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            statisticsOptionListEnum = (StatisticsOptionListEnum)Enum.Parse(typeof(StatisticsOptionListEnum), btn.Content.ToString().Replace(" ", "_"));
            //Move the GridCursor according to the client's choise
            GridCursor.Margin = new Thickness(10 + (180 * Convert.ToInt32(statisticsOptionListEnum)), 0, 0, 0);
            switch (statisticsOptionListEnum)
            {
                case StatisticsOptionListEnum.AVG_PER_WEEK:
                    (Application.Current as App).StatisticalDataViewModel.VM_SelectedValueOfStatisticsOptionListProperty = StatisticsOptionListEnum.AVG_PER_WEEK;
                    ColumnGraph.Visibility = Visibility.Visible;
                    PieGraph.Visibility = Visibility.Hidden;
                    ScrollOfAmountEventPerWeekTable.Visibility = Visibility.Hidden;
                    break;
                case StatisticsOptionListEnum.AVG_PER_MONTH:
                    (Application.Current as App).StatisticalDataViewModel.VM_SelectedValueOfStatisticsOptionListProperty = StatisticsOptionListEnum.AVG_PER_MONTH;
                    ColumnGraph.Visibility = Visibility.Visible;
                    PieGraph.Visibility = Visibility.Hidden;
                    ScrollOfAmountEventPerWeekTable.Visibility = Visibility.Hidden;
                    break;
                case StatisticsOptionListEnum.AVG_PER_WEEKDAY:
                    (Application.Current as App).StatisticalDataViewModel.VM_SelectedValueOfStatisticsOptionListProperty = StatisticsOptionListEnum.AVG_PER_WEEKDAY;
                    ColumnGraph.Visibility = Visibility.Hidden;
                    PieGraph.Visibility = Visibility.Visible;
                    ScrollOfAmountEventPerWeekTable.Visibility = Visibility.Hidden;
                    break;
                case StatisticsOptionListEnum.TOTAL_EVENTS:
                    (Application.Current as App).StatisticalDataViewModel.GraphInRangeOrNotProperty = false;
                    (Application.Current as App).StatisticalDataViewModel.VM_SelectedValueOfStatisticsOptionListProperty = StatisticsOptionListEnum.TOTAL_EVENTS;
                    ColumnGraph.Visibility = Visibility.Hidden;
                    PieGraph.Visibility = Visibility.Hidden;
                    ScrollOfAmountEventPerWeekTable.Visibility = Visibility.Visible;
                    gridRowSelected = default;
                    break;
                default:
                    break;
            }
        }
        //Function that responsible on disable all the windows etc. and then the DialogHost will open for selecting date range
        private void ShowGraphInThisRange_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            SetIsEnabled(false);
        }

        //Function that get boolean variable and set it in the mainWindow field
        private void SetIsEnabled(bool isEnable)
        {
            if (mainWindow != default)
            {
                mainWindow.IsEnabled = isEnable;
            }
        }

        //Function for setting the mainWindow field
        public void SetMainWindow(MainMenu mainWindowTemp)
        {
            this.mainWindow = mainWindowTemp;
        }

        //Function that responsible on closing the DialogHost of selecting date range and enable the window
        private void CancelAddWorkerButton_Click(object sender, RoutedEventArgs e)
        {
            DialogHost.CloseDialogCommand.Execute(null, null);
            this.IsEnabled = true;
            SetIsEnabled(true);
        }

        //Function that responsible on showing graph of sum events of a worker according to the selected row
        private void DetailsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // the original source is what was clicked.  For example a button.
                DependencyObject dep = (DependencyObject)e.OriginalSource;

                // iteratively traverse the visual tree upwards looking for the clicked row.
                while ((dep != null) && !(dep is DataGridRow))
                {
                    dep = VisualTreeHelper.GetParent(dep);
                }

                // if we found the clicked row
                if (dep != null && dep is DataGridRow)
                {
                    // get the row
                    DataGridRow gridRowSelectedNow = (DataGridRow)dep;
                    Button detailsBtnNow = (Button)sender;
                    //The rows are the same so close the graph and change the icon to ArrowDown
                    if (gridRowSelected == gridRowSelectedNow)
                    {
                        detailsBtn.Content = new MaterialDesignThemes.Wpf.PackIcon
                        { Kind = MaterialDesignThemes.Wpf.PackIconKind.ArrowDown };
                        gridRowSelected.DetailsVisibility = Visibility.Collapsed;
                        gridRowSelected = default;
                        detailsBtn = default;
                        return;
                    }
                    //The first row so open the graph and change the icon to ArrowUp
                    else if (gridRowSelectedNow != null && gridRowSelected == default)
                    {
                        detailsBtnNow.Content = new MaterialDesignThemes.Wpf.PackIcon
                        { Kind = MaterialDesignThemes.Wpf.PackIconKind.ArrowUp };
                        detailsBtn = detailsBtnNow;
                        gridRowSelected = gridRowSelectedNow;
                        gridRowSelected.DetailsVisibility = Visibility.Visible;
                    }
                    //There is already one row that still open so close the graph and open the graph of the new row and change the icons
                    else if (gridRowSelectedNow != null && gridRowSelected != default)
                    {
                        detailsBtn.Content = new MaterialDesignThemes.Wpf.PackIcon
                        { Kind = MaterialDesignThemes.Wpf.PackIconKind.ArrowDown };
                        gridRowSelected.DetailsVisibility = Visibility.Collapsed;
                        gridRowSelected = gridRowSelectedNow;
                        gridRowSelected.DetailsVisibility = Visibility.Visible;
                        detailsBtnNow.Content = new MaterialDesignThemes.Wpf.PackIcon
                        { Kind = MaterialDesignThemes.Wpf.PackIconKind.ArrowUp };
                        detailsBtn = detailsBtnNow;
                    }
                }
            }
            catch (System.Exception)
            {
            }
        }

        //Function that reponsible on updating the date range
        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            //Check if one of the selected dates are default
            if (StartDatePicker.SelectedDate == default || EndDatePicker.SelectedDate == default)
            {
                MessageBox.Show("You have to insert start date and end date");
                return;
            }
            DateTime startDate = Convert.ToDateTime((StartDatePicker.SelectedDate).ToString());
            DateTime endDate = Convert.ToDateTime((EndDatePicker.SelectedDate).ToString());
            if (startDate > endDate)
            {
                MessageBox.Show("The start date can not be bigger than end date.\nPlease insert another dates.");
                return;
            }
            switch (statisticsOptionListEnum)
            {
                case StatisticsOptionListEnum.AVG_PER_WEEK:
                    (Application.Current as App).StatisticalDataViewModel.GetAvgEventsPerWeekWithRange(startDate, endDate);
                    break;
                case StatisticsOptionListEnum.AVG_PER_MONTH:
                    (Application.Current as App).StatisticalDataViewModel.GetAvgEventsPerMonthWithRange(startDate, endDate);
                    break;
                case StatisticsOptionListEnum.AVG_PER_WEEKDAY:
                    (Application.Current as App).StatisticalDataViewModel.GetAvgEventsPerWeekdayWithRange(startDate, endDate);
                    break;
                case StatisticsOptionListEnum.TOTAL_EVENTS:
                    (Application.Current as App).StatisticalDataViewModel.GraphInRangeOrNotProperty = true;
                    (Application.Current as App).StatisticalDataViewModel.GetAmountEventsByWorkerWithRange(startDate, endDate);
                    break;
                case StatisticsOptionListEnum.Nothing:
                    break;
                default:
                    break;
            }
            DialogHost.CloseDialogCommand.Execute(null, null);
            this.IsEnabled = true;
            SetIsEnabled(true);
        }
    }
    class MyButton
    {
        public string ButtonContent { get; set; }
        public string ButtonID { get; set; }
    }
}
