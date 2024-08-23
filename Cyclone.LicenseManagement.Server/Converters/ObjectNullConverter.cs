using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyclone.LicenseManagement.Server.Converters
{
    public class ObjectNullConverter : BasicConverter
    {
        public bool IsReversed { get; set; }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool result = value == null;

            if (IsReversed)
            {
                result = !result;
            }

            return result;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}