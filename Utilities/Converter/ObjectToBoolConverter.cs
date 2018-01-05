using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Utilities.Converter
{
    public class ObjectToBoolConverter : IValueConverter
    {
        public bool IsInverse { get; set; }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool bRel = false;
            try
            {
                if (value == DependencyProperty.UnsetValue)
                {
                    return false;
                }
                if (null == value)
                    return false;
                if (value is string)
                {
                    string str = value as string;
                    if (string.IsNullOrEmpty(str))
                        return false;
                }

                bRel = true;
            }
            catch
            {
            }
            if (this.IsInverse)
                return !bRel;
            return bRel;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
