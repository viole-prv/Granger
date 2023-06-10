using System;
using System.Globalization;
using System.Windows.Data;

namespace Granger
{
    public class EnumerationDescriptionConverter : IValueConverter
    {
        public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            return Helper.GetEnumerationDescription(Value);
        }

        public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            return Value;
        }
    }
}
