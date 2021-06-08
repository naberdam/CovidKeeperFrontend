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
            /*StatisticsOptionList.Text = "";*/
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
            /*StartDatePicker.Visibility = Visibility.Hidden;
            EndDatePicker.Visibility = Visibility.Hidden;*/
            ShowGraphInThisRange.Visibility = Visibility.Hidden;
            GridCursor.Visibility = Visibility.Hidden;
            ShowGraphInThisRangeText.Visibility = Visibility.Hidden;
            statisticsOptionListEnum = StatisticsOptionListEnum.Nothing;
            /*CommentText.Visibility = Visibility.Hidden;*/
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
        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;
            if (row != null)
            {
                if ((row.DetailsVisibility == Visibility.Collapsed || row.DetailsVisibility == Visibility.Hidden))
                {
                    row.DetailsVisibility = Visibility.Visible;
                    if (lastSelectedRow != default)
                    {
                        lastSelectedRow.DetailsVisibility = Visibility.Collapsed;
                    }                    
                    lastSelectedRow = row;
                }
                else if (row.IsSelected && row.DetailsVisibility == Visibility.Visible)
                {
                    row.DetailsVisibility = Visibility.Collapsed;
                    lastSelectedRow = default;
                }
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
                    /*StartDatePicker.Visibility = Visibility.Visible;
                    EndDatePicker.Visibility = Visibility.Visible;*/
                    ShowGraphInThisRange.Visibility = Visibility.Visible;
                    CommentText.Text = "Please select a day for the week you are interested in:";
                    ShowGraphInThisRangeText.Visibility = Visibility.Visible;
                    CommentText.Visibility = Visibility.Visible;
                    break;
                case StatisticsOptionListEnum.AGE_PER_MONTH:
                    (Application.Current as App).StatisticalDataViewModel.VM_SelectedValueOfStatisticsOptionListProperty = StatisticsOptionListEnum.AGE_PER_MONTH;
                    ColumnGraph.Visibility = Visibility.Visible;
                    PieGraph.Visibility = Visibility.Hidden;
                    ScrollOfAmountEventPerWeekTable.Visibility = Visibility.Hidden;
                    /*StartDatePicker.Visibility = Visibility.Visible;
                    EndDatePicker.Visibility = Visibility.Visible;*/
                    ShowGraphInThisRange.Visibility = Visibility.Visible;
                    CommentText.Text = "Please select a day for the month you are interested in:";
                    ShowGraphInThisRangeText.Visibility = Visibility.Visible;
                    CommentText.Visibility = Visibility.Visible;
                    break;
                case StatisticsOptionListEnum.AGE_PER_WEEKDAY:
                    (Application.Current as App).StatisticalDataViewModel.VM_SelectedValueOfStatisticsOptionListProperty = StatisticsOptionListEnum.AGE_PER_WEEKDAY;
                    ColumnGraph.Visibility = Visibility.Hidden;
                    PieGraph.Visibility = Visibility.Visible;
                    ScrollOfAmountEventPerWeekTable.Visibility = Visibility.Hidden;
                    /*StartDatePicker.Visibility = Visibility.Hidden;
                    EndDatePicker.Visibility = Visibility.Hidden;*/
                    ShowGraphInThisRange.Visibility = Visibility.Hidden;
                    CommentText.Text = "Please select a day for the week you are interested in:";
                    ShowGraphInThisRangeText.Visibility = Visibility.Visible;
                    CommentText.Visibility = Visibility.Visible;
                    break;
                case StatisticsOptionListEnum.TOTAL_EVENTS:
                    (Application.Current as App).StatisticalDataViewModel.VM_SelectedValueOfStatisticsOptionListProperty = StatisticsOptionListEnum.TOTAL_EVENTS;
                    ColumnGraph.Visibility = Visibility.Hidden;
                    PieGraph.Visibility = Visibility.Hidden;
                    ScrollOfAmountEventPerWeekTable.Visibility = Visibility.Visible;
                    /*StartDatePicker.Visibility = Visibility.Hidden;
                    EndDatePicker.Visibility = Visibility.Hidden;*/
                    ShowGraphInThisRange.Visibility = Visibility.Hidden;
                    CommentText.Text = "Please select a day for the week you are interested in:";
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
            switch (statisticsOptionListEnum)
            {
                case StatisticsOptionListEnum.AGE_PER_WEEK:
                    (Application.Current as App).StatisticalDataViewModel.GetAvgEventsPerWeekWithRange(startDate, endDate);
                    break;
                case StatisticsOptionListEnum.AGE_PER_MONTH:
                    (Application.Current as App).StatisticalDataViewModel.GetAvgEventsPerMonthWithRange(startDate, endDate);
                    break;
                case StatisticsOptionListEnum.AGE_PER_WEEKDAY:
                    break;
                case StatisticsOptionListEnum.TOTAL_EVENTS:
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
