using ClashDotNetFramework.Models.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ClashDotNetFramework.Models.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool status = (bool)value;
            switch (Global.Settings.Theme)
            {
                case ThemeType.Classic:
                    if (status)
                    {
                        return new SolidColorBrush(Color.FromArgb((byte)Math.Round(0.9 * 255), Global.ProxyColor.R, Global.ProxyColor.G, Global.ProxyColor.B));
                    }
                    else
                    {
                        return new SolidColorBrush(Color.FromArgb((byte)Math.Round(0.6 * 255), Global.ProxyColor.R, Global.ProxyColor.G, Global.ProxyColor.B));
                    }
                case ThemeType.Modern:
                    if (status)
                    {
                        return new LinearGradientBrush(Color.FromRgb(87, 190, 252), Color.FromRgb(44, 138, 248), new Point(0, 0), new Point(1, 1));
                    }
                    else
                    {
                        return new LinearGradientBrush(Color.FromArgb((byte)Math.Round(0.5 * 255), 87, 190, 252), Color.FromArgb((byte)Math.Round(0.5 * 255), 44, 138, 248), new Point(0, 0), new Point(1, 1));
                    }
                case ThemeType.Dark:
                    if (status)
                    {
                        return new SolidColorBrush(Color.FromRgb(66, 66, 78));
                    }
                    else
                    {
                        return new SolidColorBrush(Color.FromRgb(50, 50, 63));
                    }
                default:
                    return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
