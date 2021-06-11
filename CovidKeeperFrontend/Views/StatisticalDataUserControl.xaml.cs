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
        DataGridRow lastSelectedRow = default;
        MainWindowTemp mainWindowTemp = default;
        StatisticsOptionListEnum statisticsOptionListEnum = StatisticsOptionListEnum.Nothing;
        Button detailsBtn = default;
        DataGridRow gridRowSelected = default;
        public enum StatisticsOptionListEnum
        {
            AGE_PER_WEEK = 0,
            AGE_PER_MONTH = 1,
            AGE_PER_WEEKDAY = 2,
            TOTAL_EVENTS = 3,
            Nothing = 4
        }
        public StatisticalDataUserControl()
        {
            InitializeComponent();
        }

        public void ClearFields()
        {
            List<StatisticsOptionListEnum> buttonContentList = new List<StatisticsOptionListEnum>() { StatisticsOptionListEnum.AGE_PER_WEEK, 
                StatisticsOptionListEnum.AGE_PER_MONTH, StatisticsOptionListEnum.AGE_PER_WEEKDAY, StatisticsOptionListEnum.TOTAL_EVENTS };
            List<MyButton> buttons = new List<MyButton>();
            int buttonId = 1;
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
            if (lastSelectedRow != default)
            {
                lastSelectedRow.DetailsVisibility = Visibility.Collapsed;
            }
            AmountEventPerWeekTable.SelectedIndex = -1;
            lastSelectedRow = default;
            ColumnGraph.Visibility = Visibility.Hidden;
            PieGraph.Visibility = Visibility.Hidden;
            ScrollOfAmountEventPerWeekTable.Visibility = Visibility.Hidden;
            ShowGraphInThisRange.Visibility = Visibility.Hidden;
            GridCursor.Visibility = Visibility.Hidden;
            ShowGraphInThisRangeText.Visibility = Visibility.Hidden;
            statisticsOptionListEnum = StatisticsOptionListEnum.Nothing;
            detailsBtn = default;
            gridRowSelected = default;
        }

        private void AmountEventPerWeekTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            DataRowView rowSelected = gd.SelectedItem as DataRowView;
            if (rowSelected != null)
            {
                (Application.Current as App).StatisticalDataViewModel.VM_IdWorkerForLineGraphProperty = rowSelected["Id_worker"].ToString();
            }            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            statisticsOptionListEnum = (StatisticsOptionListEnum)Enum.Parse(typeof(StatisticsOptionListEnum), btn.Content.ToString().Replace(" ", "_"));
            GridCursor.Margin = new Thickness(10 + (180 * Convert.ToInt32(statisticsOptionListEnum)), 0, 0, 0);
            GridCursor.Visibility = Visibility.Visible;
            switch (statisticsOptionListEnum)
            {
                case StatisticsOptionListEnum.AGE_PER_WEEK:
                    (Application.Current as App).StatisticalDataViewModel.VM_SelectedValueOfStatisticsOptionListProperty = StatisticsOptionListEnum.AGE_PER_WEEK;
                    ColumnGraph.Visibility = Visibility.Visible;
                    PieGraph.Visibility = Visibility.Hidden;
                    ScrollOfAmountEventPerWeekTable.Visibility = Visibility.Hidden;
                    ShowGraphInThisRange.Visibility = Visibility.Visible;
                    //CommentText.Text = "Please select a day for the week you are interested in:";
                    ShowGraphInThisRangeText.Visibility = Visibility.Visible;
                    CommentText.Visibility = Visibility.Visible;
                    break;
                case StatisticsOptionListEnum.AGE_PER_MONTH:
                    (Application.Current as App).StatisticalDataViewModel.VM_SelectedValueOfStatisticsOptionListProperty = StatisticsOptionListEnum.AGE_PER_MONTH;
                    ColumnGraph.Visibility = Visibility.Visible;
                    PieGraph.Visibility = Visibility.Hidden;
                    ScrollOfAmountEventPerWeekTable.Visibility = Visibility.Hidden;
                    ShowGraphInThisRange.Visibility = Visibility.Visible;
                    //CommentText.Text = "Please select a day for the month you are interested in:";
                    ShowGraphInThisRangeText.Visibility = Visibility.Visible;
                    CommentText.Visibility = Visibility.Visible;
                    break;
                case StatisticsOptionListEnum.AGE_PER_WEEKDAY:
                    (Application.Current as App).StatisticalDataViewModel.VM_SelectedValueOfStatisticsOptionListProperty = StatisticsOptionListEnum.AGE_PER_WEEKDAY;
                    ColumnGraph.Visibility = Visibility.Hidden;
                    PieGraph.Visibility = Visibility.Visible;
                    ScrollOfAmountEventPerWeekTable.Visibility = Visibility.Hidden;
                    ShowGraphInThisRange.Visibility = Visibility.Visible;
                    //CommentText.Text = "Please select a day for the week you are interested in:";
                    ShowGraphInThisRangeText.Visibility = Visibility.Visible;
                    CommentText.Visibility = Visibility.Visible;
                    break;
                case StatisticsOptionListEnum.TOTAL_EVENTS:
                    (Application.Current as App).StatisticalDataViewModel.VM_SelectedValueOfStatisticsOptionListProperty = StatisticsOptionListEnum.TOTAL_EVENTS;
                    ColumnGraph.Visibility = Visibility.Hidden;
                    PieGraph.Visibility = Visibility.Hidden;
                    ScrollOfAmountEventPerWeekTable.Visibility = Visibility.Visible;
                    ShowGraphInThisRange.Visibility = Visibility.Visible;
                    //CommentText.Text = "Please select a day for the week you are interested in:";
                    ShowGraphInThisRangeText.Visibility = Visibility.Visible;
                    CommentText.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
        }

        private void ShowGraphInThisRange_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            SetIsEnabled(false);
        }
        private void SetIsEnabled(bool isEnable)
        {
            if (mainWindowTemp != default)
            {
                mainWindowTemp.IsEnabled = isEnable;
            }
        }
        public void SetMainWindow(MainWindowTemp mainWindowTemp)
        {
            this.mainWindowTemp = mainWindowTemp;
        }

        private void CancelAddWorkerButton_Click(object sender, RoutedEventArgs e)
        {
            DialogHost.CloseDialogCommand.Execute(null, null);
            this.IsEnabled = true;
            SetIsEnabled(true);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (StartDatePicker.SelectedDate == default && EndDatePicker.SelectedDate == default)
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
                case StatisticsOptionListEnum.AGE_PER_WEEK:
                    (Application.Current as App).StatisticalDataViewModel.GetAvgEventsPerWeekWithRange(startDate, endDate);
                    break;
                case StatisticsOptionListEnum.AGE_PER_MONTH:
                    (Application.Current as App).StatisticalDataViewModel.GetAvgEventsPerMonthWithRange(startDate, endDate);
                    break;
                case StatisticsOptionListEnum.AGE_PER_WEEKDAY:
                    (Application.Current as App).StatisticalDataViewModel.GetAvgEventsPerWeekdayWithRange(startDate, endDate);
                    break;
                case StatisticsOptionListEnum.TOTAL_EVENTS:
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
        private void DetailsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // the original source is what was clicked.  For example 
                // a button.
                DependencyObject dep = (DependencyObject)e.OriginalSource;

                // iteratively traverse the visual tree upwards looking for
                // the clicked row.
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
                    if (gridRowSelected == gridRowSelectedNow)
                    {
                        detailsBtn.Content = new MaterialDesignThemes.Wpf.PackIcon
                        { Kind = MaterialDesignThemes.Wpf.PackIconKind.ArrowDown };
                        gridRowSelected.DetailsVisibility = Visibility.Collapsed;
                        gridRowSelected = default;
                        detailsBtn = default;
                        return;
                    }
                    else if (gridRowSelectedNow != null && gridRowSelected == default)
                    {
                        detailsBtnNow.Content = new MaterialDesignThemes.Wpf.PackIcon
                        { Kind = MaterialDesignThemes.Wpf.PackIconKind.ArrowUp };
                        detailsBtn = detailsBtnNow;
                        gridRowSelected = gridRowSelectedNow;
                        gridRowSelected.DetailsVisibility = Visibility.Visible;
                    }
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

    }
    class MyButton
    {
        public string ButtonContent { get; set; }
        public string ButtonID { get; set; }
    }
}
