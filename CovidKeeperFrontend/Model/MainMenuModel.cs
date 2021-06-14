using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovidKeeperFrontend.Model
{
    public class MainMenuModel : AbstractModel
    {
        public NotifyTaskCompletion<int> UpdateTimeBreakForMailsAsync { get; private set; }
        private int minutesBreakForMails;
        public int MinutesBreakForMailsProperty
        {
            get { return minutesBreakForMails; }
            set
            {
                if (minutesBreakForMails != value)
                {
                    minutesBreakForMails = value;
                    UpdateTimeBreakForMailsAsync = new NotifyTaskCompletion<int>(UpdateTimeBreakForMails(minutesBreakForMails));
                }
            }
        }
        public override async Task<int> RefreshDataAsync()
        {
            await Task.Run(() => {});
            return default;
        }
        private async Task<int> UpdateTimeBreakForMails(int minutesBreakToChange)
        {
            await azureSingleton.UpdateTimeBreakForMails(minutesBreakToChange);
            return default;
        }
    }
}
