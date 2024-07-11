using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WordleCheatWPF
{
    public class GuessRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
            foreach (char c in (string)value)
            {
                if (!char.IsLetter(c))
                    return new ValidationResult(false, $"{c} is not a letter");
            }
            return ValidationResult.ValidResult;
            }
            catch (Exception e)
            {
                return new ValidationResult(false, $"Illegal characters or {e.Message}");
            }
        }
    }
}
