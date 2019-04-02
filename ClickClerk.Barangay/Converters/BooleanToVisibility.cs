using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickClerk.Barangay.Converters
{
    class BooleanToVisibility:ConverterBase
    {
        public BooleanToVisibility(){}

        private static BooleanToVisibility _instance;
        public static BooleanToVisibility Instance => _instance ?? (_instance = new BooleanToVisibility());

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var invert = bool.Parse(parameter?.ToString()??"False");
            if ((value is bool b && b))
                return invert ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
            return !invert ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
        }
    }
}
