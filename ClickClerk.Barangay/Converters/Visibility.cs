using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickClerk.Barangay.Converters
{
    class Visibility:ConverterBase
    {
        private Visibility(){}

        private static Visibility _instance;
        public static Visibility Instance => _instance ?? (_instance = new Visibility());

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var invert = bool.Parse(parameter?.ToString()??"False");
            if ((value is bool b && b))
                return invert ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
            return !invert ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
        }
    }
}
