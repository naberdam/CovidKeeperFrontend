using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovidKeeperFrontend.HelperClasses
{
    public static class GlobalVariables
    {
        public const string DBO_NAME = "dbo";

        public const string WORKERS_TABLE_NAME = "Workers";
        public const string FULL_NAME_FIELD = "FullName";
        public const string EMAIL_ADDRESS_FIELD = "Email_address";
        public const string ID_FIELD = "Id";
        public const string IMAGE_FIELD = "Image";
        public const string IMAGE_BOX_FIELD = "Image_checkbox";

        public const string HISTORY_EVENTS_TABLE_NAME = "History_Events";
        public const string ID_WORKER_FIELD = "Id_worker";
        public const string TIME_OF_EVENT_FIELD = "Time_of_event";
        
        public const string STARTER_TABLE_NAME = "Starter";
        public const string HANDLE_STARTER_FIELD = "Handle";

        public const string ANALAYZER_CONFIG_TABLE_NAME = "Analayzer_config";
        public const string HANDLE_ANALAYZER_FIELD = "Handle";

        public const string MANAGER_CONFIG_TABLE_NAME = "Manager_Config";
        public const string HANDLE_MANAGER_FIELD = "Handle";
        public const string MINUTES_BETWEEN_MAILS_FIELD = "Minutes_between_mails";

        public const string FILES_FOLDER_PATH = "CovidKeeperFrontend\\Files\\";
    }
}
