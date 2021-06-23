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
            //Check if it is null or just white space
            if (string.IsNullOrWhiteSpace(emailAddress))
            {
                return new ValidationResult(false, $"Email can not be empty");
            }
            //Check if there are special characters
            if (Regex.IsMatch(emailAddress, "[!#$%^&*)(;,'\\\\/~`|}{\\-\\+\\[\\]\\?]"))
            {
                return new ValidationResult(false, $"Can not use special characters");
            }
            //Check the amount of the char '@' because it can be only one '@'
            int count = emailAddress.Count(y => y == '@');
            if (count >= 2)
            {
                return new ValidationResult(false, $"@ can only be once");
            }            
            //Check if the string is not match to the email pattern
            if (!Regex.IsMatch(emailAddress, "^([a-zA-Z._0-9]+)@([a-zA-Z_0-9]+)\\.([a-zA-Z._0-9]+)$"))
            {
                return new ValidationResult(false, $"Invalid email address");
            }
            return new ValidationResult(true, null);
        }
    }
}
