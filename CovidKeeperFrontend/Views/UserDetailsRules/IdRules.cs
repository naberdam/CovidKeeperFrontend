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
    public class IdRules : ValidationRule
    {
        public int MinimumDigits { get; set; }
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string id = value as string;
            //Check if it is null or just white space
            if (string.IsNullOrWhiteSpace(id))
            {
                return new ValidationResult(false, $"Id can not be empty");
            }
            //Check if the only characters are digits
            if (Regex.IsMatch(id, "[a-zA-Z ,`~!@#$%^&*)(\\+\\-\\/}{\\[\\]\\.\\?|]"))
            {
                return new ValidationResult(false, $"Only digits are allowed");
            }
            //Check if this string has the minimum length
            if (id.Length < MinimumDigits)
            {
                return new ValidationResult(false, $"Id at least {MinimumDigits} digits");
            }

            return new ValidationResult(true, null);
        }
    }
}
