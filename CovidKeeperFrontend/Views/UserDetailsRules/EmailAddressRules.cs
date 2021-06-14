using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace CovidKeeperFrontend.Views.UserDetailsRules
{
    public class EmailAddressRules : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string emailAddress = value as string;
            if (string.IsNullOrWhiteSpace(emailAddress))
            {
                return new ValidationResult(false, $"Email can not be empty");
            }
            if (Regex.IsMatch(emailAddress, "[!#$%^&*)(;,'\\\\/~`|}{\\-\\+\\[\\]\\?]"))
            {
                return new ValidationResult(false, $"Can not use special characters");
            }
            int count = emailAddress.Count(y => y == '@');
            if (count >= 2)
            {
                return new ValidationResult(false, $"@ can only be once");
            }            
            if (!Regex.IsMatch(emailAddress, "^([a-zA-Z._0-9]+)@([a-zA-Z_0-9]+)\\.([a-zA-Z._0-9]+)$"))
            {
                return new ValidationResult(false, $"Invalid email address");
            }
            return new ValidationResult(true, null);
        }
    }
}
