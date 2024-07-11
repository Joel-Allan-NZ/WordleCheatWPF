using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using WordleCheat.Logic;

namespace WordleCheatWPF
{
    public class LetterGuessCorrectnessToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value != null && value is LetterGuessCorrectness)
            {
                var lgc = (LetterGuessCorrectness)value;

                if (lgc == LetterGuessCorrectness.NotIncuded)
                    
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#939598"));

                if (lgc == LetterGuessCorrectness.IncorrectLocation)
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#b59f3b"));

                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6aaa64"));

            }
            return Brushes.Gray;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
