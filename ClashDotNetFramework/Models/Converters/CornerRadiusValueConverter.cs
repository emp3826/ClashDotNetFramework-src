using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ClashDotNetFramework.Models.Converters
{
    public class CornerRadiusValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var radius = Double.Parse(value.ToString(), culture);
            if (parameter != null)
            {
                radius *= Double.Parse(parameter.ToString(), culture);
            }
            return new CornerRadius(radius);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
