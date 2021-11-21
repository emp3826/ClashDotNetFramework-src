using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace ClashDotNetFramework.Models.Converters
{
    public class LatencyToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            long latency = (long)value;
            switch (latency)
            {
                case 0:
                case -1:
                    return new SolidColorBrush(Color.FromRgb(255, 0, 0));
                case -2:
                    return new SolidColorBrush(Color.FromRgb(255, 255, 255));
                default:
                    return new SolidColorBrush(Color.FromRgb(255, 255, 255));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
