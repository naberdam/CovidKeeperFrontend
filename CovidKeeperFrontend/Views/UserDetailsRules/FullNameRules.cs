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
        public int MaximumCharacters { get; set; }
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string fullName = value as string;
            //Check if it is null or just white space
            if (string.IsNullOrWhiteSpace(fullName))
            {
                return new ValidationResult(false, $"Full name can not be empty");
            }
            //Check if the only characters are letters
            if (Regex.IsMatch(fullName, "[0-9,`~!@#$%^&*)(\\+\\-\\/}{\\[\\]\\.\\?|]"))
            {
                return new ValidationResult(false, $"Only letters are allowed");
            }
            //Check if full name contains space
            if (!fullName.Contains(' '))
            {
                return new ValidationResult(false, $"Full name must have at least two words");
            }
            //Check if this string has the maximum length
            if (fullName.Length > MaximumCharacters)
            {
                return new ValidationResult(false, $"At most {MaximumCharacters} characters");
            }
            //Check if each word in this string has the minimum length
            string[] fullNameArray = fullName.Split();
            foreach (string word in fullNameArray)
            {
                if (word.Length < MinimumCharactersForEachWord)
                {
                    return new ValidationResult(false, $"At least {MinimumCharactersForEachWord} characters in each word");
                }
            }
            /*if (fullNameArray[0].Length < MinimumCharactersForEachWord || fullNameArray[1].Length < MinimumCharactersForEachWord)
            {
                return new ValidationResult(false, $"At least {MinimumCharactersForEachWord} characters for each word");
            }*/
            return new ValidationResult(true, null);
        }
    }
}
