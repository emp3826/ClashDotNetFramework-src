using ClashDotNetFramework.Models.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ClashDotNetFramework.Models.Converters
{
    public class ProfileTypeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ProfileType profileType = (ProfileType)value;
            switch (profileType)
            {
                case ProfileType.Local:
                    return "Local File";
                case ProfileType.Remote:
                    return "Remote File";
                default:
                    return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
