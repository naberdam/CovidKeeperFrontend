using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovidKeeperFrontend.Model
{
    //This class is the model of MainMenu
    public class MainMenuModel : AbstractModel
    {
        public NotifyTaskCompletion<int> UpdateTimeBreakForMailsAsync { get; private set; }
        //Property that defines that minutes break between sending mails to the workers in case the workers has some events in the same day
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
        //Override function for refreshing MainMenu, but there is no need in this here
        public override async Task<int> RefreshDataAsync()
        {
            await Task.Run(() => {});
            return default;
        }
        //Function that updates the time break between sending mails
        private async Task<int> UpdateTimeBreakForMails(int minutesBreakToChange)
        {
            await azureSingleton.UpdateTimeBreakForMails(minutesBreakToChange);
            return default;
        }
    }
}
