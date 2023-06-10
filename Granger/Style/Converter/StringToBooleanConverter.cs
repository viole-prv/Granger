using System;
using System.Globalization;
using System.Windows.Data;

namespace Granger
{
    public class StringToBooleanConverter : IValueConverter
    {
        public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            if (Value == null || string.IsNullOrEmpty(Value.ToString()))
            {
                return false;
            }

            return true;
        }

        public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            return Value;
        }
    }
}
