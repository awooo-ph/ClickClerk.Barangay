using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickClerk.Barangay.Converters
{
    class BooleanToOpacity:ConverterBase
    {
        private BooleanToOpacity(){}

        private static BooleanToOpacity _instance;
        public static BooleanToOpacity Instance => _instance ?? (_instance = new BooleanToOpacity());

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var param = parameter.ToString().Split(new []{' '});
            if (value is bool v && v) 
                return double.Parse(param[1]);
            return double.Parse(param[0]);
        }
    }
}
