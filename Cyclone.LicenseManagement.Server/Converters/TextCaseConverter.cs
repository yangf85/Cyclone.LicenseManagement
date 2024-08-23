using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyclone.LicenseManagement.Server.Converters
{
    public class TextCaseConverter : BasicConverter
    {
        public bool IsUpperCase { get; set; }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string strValue)
            {
                return IsUpperCase ? strValue.ToUpper(culture) : strValue.ToLower(culture);
            }
            return value;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string strValue)
            {
                return IsUpperCase ? strValue.ToLower(culture) : strValue.ToUpper(culture);
            }
            return value;
        }
    }
}