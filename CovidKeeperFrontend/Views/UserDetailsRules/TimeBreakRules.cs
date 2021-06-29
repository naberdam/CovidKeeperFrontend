using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CovidKeeperFrontend.Views.UserDetailsRules
{
    public class TimeBreakRules : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string timeBreakString = value as string;
            //Check if it is null or just white space
            if (string.IsNullOrWhiteSpace(timeBreakString))
            {
                return new ValidationResult(false, $"Minutes can not be empty");
            }
            //Check if the only characters are digits
            if (Regex.IsMatch(timeBreakString, "[a-zA-Z ,`~!@#$%^&*)(\\+\\-\\/}{\\[\\]\\.\\?|]"))
            {
                return new ValidationResult(false, $"Only digits are allowed");
            }
            if (Regex.IsMatch(timeBreakString, "^0"))
            {
                return new ValidationResult(false, $"Minutes can't start with 0");
            }
            return new ValidationResult(true, null);
        }
    }
}
