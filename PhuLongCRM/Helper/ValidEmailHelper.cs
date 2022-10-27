using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PhuLongCRM.Helper
{
    public class ValidEmailHelper
    {
        public static bool CheckValidEmail(string email)
        {
            string validEmailPattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
            + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
            + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";
            var ValidEmailRegex = new Regex(validEmailPattern, RegexOptions.IgnoreCase);
            return ValidEmailRegex.IsMatch(email.ToLower());
        }
    }
}
