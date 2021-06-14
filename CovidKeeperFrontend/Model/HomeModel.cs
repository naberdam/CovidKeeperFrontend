using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovidKeeperFrontend.Model
{
    public class HomeModel : AbstractModel
    {
        private string howManyWorkersWithoutMask = "";
        public string HowManyWorkersWithoutMaskProperty
        {
            get
            {
                return howManyWorkersWithoutMask;
            }
            set
            {
                if (howManyWorkersWithoutMask != value)
                {
                    howManyWorkersWithoutMask = value;
                    NotifyPropertyChanged("HowManyWorkersWithoutMaskProperty");
                }
            }
        }
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

        public enum HandleFlag
        {
            Close = 1,
            Start = 0
        }

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
            RefreshData = new NotifyTaskCompletion<int>(RefreshDataAsync());
        }


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
                        HowManyWorkersWithoutMaskProperty = counter[0].ToString() + "/" + countAllWorkers;
                    }
                }
            });
        }
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

        public override async Task<int> RefreshDataAsync()
        {
            await CountWorkersWithoutMaskToday();
            await GetActiveButtonContent();
            await PercentageWorkersWithoutMaskTodayPerYesterday();
            await GetAmountEventsToday();
            return default;
        }
        public async Task StartOrCloseProgram()
        {
            if (ActiveButtonContentProperty == HandleFlag.Start)
            {
                await UpdateHandleInStarter("0", "1");
                ActiveButtonContentProperty = HandleFlag.Close;
            }
            else
            {
                await UpdateHandleInStarter("1", "0");
                ActiveButtonContentProperty = HandleFlag.Start;
            }
        }
        private async Task UpdateHandleInStarter(string valueInTable, string valueToUpdate)
        {
            string updateQuery = @"UPDATE [dbo].[Starter] SET Handle = @Handle where Handle = " + valueInTable;
            Dictionary<string, string> fieldNameToValueDict = new Dictionary<string, string> 
            { 
                { "@Handle", valueToUpdate } 
            };
            //await UpdateWithOneParameter(updateQuery, "@Handle", valueToUpdate);
            await QueryDatabaseWithDict(updateQuery, fieldNameToValueDict);
        }
    }
}
