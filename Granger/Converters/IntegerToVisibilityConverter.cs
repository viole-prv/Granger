using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Granger
{
    public class IntegerToVisibilityConverter : IValueConverter
    {
        public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            return System.Convert.ToInt32(Value) > 0
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            return Value;
        }
    }
}
