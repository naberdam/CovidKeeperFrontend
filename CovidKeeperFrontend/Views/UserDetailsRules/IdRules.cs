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
            if (string.IsNullOrWhiteSpace(id))
            {
                return new ValidationResult(false, $"Id can not be empty");
            }
            if (Regex.IsMatch(id, "[a-zA-Z ,`~!@#$%^&*)(\\+\\-\\/}{\\[\\]\\.\\?|]"))
            {
                return new ValidationResult(false, $"Only digits are allowed");
            }
            if (id.Length < MinimumDigits)
            {
                return new ValidationResult(false, $"Id at least {MinimumDigits} digits");
            }

            return new ValidationResult(true, null);
        }
    }
}
