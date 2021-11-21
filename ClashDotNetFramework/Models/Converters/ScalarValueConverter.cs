using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ClashDotNetFramework.Models.Converters
{
    public class ScalarValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var oldValue = Double.Parse(value.ToString(), culture);
            if (parameter != null)
            {
                oldValue *= Double.Parse(parameter.ToString(), culture);
            }
            return oldValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var oldValue = Double.Parse(value.ToString(), culture);
            if (parameter != null)
            {
                oldValue /= Double.Parse(parameter.ToString(), culture);
            }
            return oldValue;
        }
    }
}
