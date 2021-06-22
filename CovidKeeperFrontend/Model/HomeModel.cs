using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovidKeeperFrontend.Model
{
    //This class is the model of HomeUserControl
    public class HomeModel : AbstractModel
    {
        //Property that defines how many workers without mask today out of all workers
        private string howManyWorkersWithoutTodayMask = "";
        public string HowManyWorkersWithoutMaskTodayProperty
        {
            get { return howManyWorkersWithoutTodayMask; }
            set
            {
                if (howManyWorkersWithoutTodayMask != value)
                {
                    howManyWorkersWithoutTodayMask = value;
                    NotifyPropertyChanged("HowManyWorkersWithoutMaskProperty");
                }
            }
        }

        //Property that defines how many events were today
        private string howManyEventsToday;
        public string HowManyEventsTodayProperty
        {
            get { return howManyEventsToday; }
            set
            {
                if (howManyEventsToday != value)
                {
                    howManyEventsToday = value;
                    NotifyPropertyChanged("HowManyEventsTodayProperty");
                }
            }
        }

        //Property that defines the percentage workers without mask today per yesterday
        private float percentageWorkersWithoutMaskTodayPerYesterday = 0;
        public float PercentageWorkersWithoutMaskTodayPerYesterdayProperty
        {
            get { return percentageWorkersWithoutMaskTodayPerYesterday; }
            set
            {
                if (percentageWorkersWithoutMaskTodayPerYesterday != value)
                {
                    percentageWorkersWithoutMaskTodayPerYesterday = value;
                    NotifyPropertyChanged("PercentageWorkersWithoutMaskTodayPerYesterdayProperty");
                }
            }
        }

        //Enum that represents the status of the back of the program if it is closed or not
        public enum HandleFlag
        {
            Stop = 1,
            Start = 0
        }

        //Property that defines the status of the active button content to be "close" or "start"
        private HandleFlag activeButtonContent;
        public HandleFlag ActiveButtonContentProperty
        {
            get { return activeButtonContent; }
            set
            {
                if (activeButtonContent != value)
                {
                    activeButtonContent = value;
                    NotifyPropertyChanged("ActiveButtonContentProperty");
                }
            }
        }
        public NotifyTaskCompletion<int> RefreshData { get; private set; }

        public HomeModel()
        {
            //Refresh data
            RefreshData = new NotifyTaskCompletion<int>(RefreshDataAsync());
        }

        //Function that gets the counter of the workers without mask today and updates the HowManyWorkersWithoutMaskProperty
        private async Task CountWorkersWithoutMaskToday()
        {
            string countAllWorkers = await CountWorkers();
            await Task.Run(() =>
            {
                string countQuery = "SELECT COUNT( DISTINCT[Id_worker]) as num_bad_workers FROM[dbo].[History_Events] WHERE DATEDIFF(day, Time_of_event, GETDATE())<= 0;";
                object[] counter = QuerySelectOfOneRow(countQuery);
                if (counter != null)
                {
                    if (countAllWorkers != null)
                    {
                        HowManyWorkersWithoutMaskTodayProperty = counter[0].ToString() + "/" + countAllWorkers;
                    }
                }
            });
        }
        //Function that gets the status of the back of the program and updates the ActiveButtonContentProperty
        private async Task GetActiveButtonContent()
        {
            await Task.Run(() =>
            {
                string handleQuery = "SELECT Handle from [dbo].[Starter]";
                object[] handle = QuerySelectOfOneRow(handleQuery);
                if (handle != null)
                {
                    HandleFlag startOrClose = (HandleFlag)Enum.Parse(typeof(HandleFlag), handle[0].ToString());
                    ActiveButtonContentProperty = startOrClose;
                }
            });
        }
        //Function that gets the amount events today and updates the HowManyEventsTodayProperty
        private async Task GetAmountEventsToday()
        {
            await Task.Run(() =>
            {
                string countQuery = "SELECT COUNT(Id_worker) FROM [dbo].[History_Events] where (DATEDIFF(d, Time_of_event, GETDATE()) = 0);";
                object[] counter = QuerySelectOfOneRow(countQuery);
                if (counter != null)
                {
                    HowManyEventsTodayProperty = counter[0].ToString();
                }
            });
        }
        //Function that returns how many workers
        private async Task<string> CountWorkers()
        {
            return await Task.Run(() =>
            {
                string countQuery = "SELECT Count(Id) FROM [dbo].[Workers];";
                object[] counter = QuerySelectOfOneRow(countQuery);
                if (counter != null)
                {
                    return counter[0].ToString();
                }
                return null;
            });

        }
        //Function that gets the percentage workers without mask today per yesterday and updates the PercentageWorkersWithoutMaskTodayPerYesterdayProperty
        public async Task PercentageWorkersWithoutMaskTodayPerYesterday()
        {
            await Task.Run(() =>
            {
                string todayPerYesterdayQuery = "select " +
                "(SELECT COUNT( DISTINCT[Id_worker]) as num_bad_workers " +
                "FROM[dbo].[History_Events] " +
                "WHERE DATEDIFF(day, Time_of_event, GETDATE())<= 0) as countToday, " +
                "(SELECT COUNT(DISTINCT[Id_worker]) as num_bad_workers " +
                "FROM[dbo].[History_Events] " +
                "WHERE DATEDIFF(day, Time_of_event, GETDATE()-1)<= 0) as countYesterday; ";
                object[] todayPerYesterday = QuerySelectOfOneRow(todayPerYesterdayQuery);
                if (todayPerYesterday != null)
                {
                    int countToday = Convert.ToInt32(todayPerYesterday[0]);
                    int countYesterday = Convert.ToInt32(todayPerYesterday[1]);
                    if (countYesterday != 0)
                    {
                        PercentageWorkersWithoutMaskTodayPerYesterdayProperty = (countToday / countYesterday) * 100 - 100;
                    }
                }
            });
        }
        //Override function for refreshing HomeUserControl
        public override async Task<int> RefreshDataAsync()
        {
            await CountWorkersWithoutMaskToday();
            await GetActiveButtonContent();
            await PercentageWorkersWithoutMaskTodayPerYesterday();
            await GetAmountEventsToday();
            return default;
        }
        //Function that changes the flag of start or close that responsible of starting/closing the backend of the program
        public async Task StartOrCloseProgram()
        {
            if (ActiveButtonContentProperty == HandleFlag.Start)
            {
                //change the value from 0 to 1
                await UpdateHandleInStarter("0", "1");
                ActiveButtonContentProperty = HandleFlag.Stop;
            }
            else
            {
                //change the value from 1 to 0
                await UpdateHandleInStarter("1", "0");
                ActiveButtonContentProperty = HandleFlag.Start;
            }
        }
        //Function that updates the flag in Starter table
        private async Task UpdateHandleInStarter(string valueInTable, string valueToUpdate)
        {
            string updateQuery = @"UPDATE [dbo].[Starter] SET Handle = @Handle where Handle = " + valueInTable;
            Dictionary<string, string> fieldNameToValueDict = new Dictionary<string, string> 
            { 
                { "@Handle", valueToUpdate } 
            };
            await QueryDatabaseWithDict(updateQuery, fieldNameToValueDict);
        }
    }
}
