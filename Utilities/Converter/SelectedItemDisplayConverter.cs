using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Utilities.Converter
{
    /// <summary>
    /// This converter supports string/dictionary(key/value)
    /// </summary>
    public class SelectedItemDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string returnedValue = string.Empty;
            if (null == value)
                return returnedValue;
            if (value == DependencyProperty.UnsetValue)
                return returnedValue;

            var type = value.GetType();
            if (type.IsGenericType)
            {
                 var valueProperty = type.GetProperty("Value");
                if (null != valueProperty)
                {
                    var valueObj = valueProperty.GetValue(value, null);
                    returnedValue = (null == valueObj) ? string.Empty : valueObj.ToString();
                }
            }
            else
            {
                returnedValue = value.ToString();
            }
            return returnedValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
