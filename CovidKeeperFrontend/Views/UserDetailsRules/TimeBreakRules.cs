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
            if (string.IsNullOrWhiteSpace(timeBreakString))
            {
                return new ValidationResult(false, $"Minutes can not be empty");
            }
            if (Regex.IsMatch(timeBreakString, "[a-zA-Z ,`~!@#$%^&*)(\\+\\-\\/}{\\[\\]\\.\\?|]"))
            {
                return new ValidationResult(false, $"Only digits are allowed");
            }
            int timeBreak = Int32.Parse(timeBreakString);
            if (timeBreak < 0)
            {
                return new ValidationResult(false, $"Only a positive number is allowed");
            }
            return new ValidationResult(true, null);
        }
    }
}
