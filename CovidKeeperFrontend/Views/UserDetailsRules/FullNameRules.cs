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
    public class FullNameRules : ValidationRule
    {
        public int MinimumCharactersForEachWord { get; set; }
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string fullName = value as string;
            if (string.IsNullOrWhiteSpace(fullName))
            {
                return new ValidationResult(false, $"Full name can not be empty");
            }
            if (Regex.IsMatch(fullName, "[0-9,`~!@#$%^&*)(\\+\\-\\/}{\\[\\]\\.\\?|]"))
            {
                return new ValidationResult(false, $"Only letters are allowed");
            }
            if (!fullName.Contains(' '))
            {
                return new ValidationResult(false, $"Full name must have at least two words");
            }            
            string[] fullNameArray = fullName.Split();
            if (fullNameArray[0].Length < MinimumCharactersForEachWord || fullNameArray[1].Length < MinimumCharactersForEachWord)
            {
                return new ValidationResult(false, $"At least {MinimumCharactersForEachWord} characters for each word");
            }

            return new ValidationResult(true, null);
        }
    }
}
