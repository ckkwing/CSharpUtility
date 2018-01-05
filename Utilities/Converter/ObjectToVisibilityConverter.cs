using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Utilities.Converter
{
    public class ObjectToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">if "Hidden" will convert to Visibility.Hidden else Visibility.Collapsed .</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool result = false;
            if (value == null)
            {
            }
            else if (value.GetType() == typeof(bool))
            {
                result = (bool)value;
            }
            else if (value.GetType() == typeof(string))
            {
                result = !string.IsNullOrEmpty((string)value);
            }
            else if (value.GetType() == typeof(Visibility))
            {
                result = ((Visibility)value) == Visibility.Visible;
            }
            else
            {
                result = true;
            }

            string convertWay = (string)parameter;
            Visibility visibility = result ? Visibility.Visible : Visibility.Collapsed;
            if (string.IsNullOrEmpty(convertWay))
            {
            }
            else if (convertWay == "Hidden"
                || convertWay == "VisibleHidden"
                || convertWay == "Visible2Hidden"
                || convertWay == "VH"
                || convertWay == "V2H")
            {
                visibility = result ? Visibility.Visible : Visibility.Hidden;
            }
            else if (convertWay == "HiddenVisible"
                || convertWay == "Hidden2Visible"
                || convertWay == "HV"
                || convertWay == "H2V")
            {
                visibility = result ? Visibility.Hidden : Visibility.Visible;
            }
            else if (convertWay == "CollapsedVisible"
                || convertWay == "Collapsed2Visible"
                || convertWay == "CV"
                || convertWay == "C2V")
            {
                visibility = result ? Visibility.Collapsed : Visibility.Visible;
            }

            if (targetType == typeof(bool))
            {
                return (bool)(visibility == Visibility.Visible);
            }

            return visibility;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return null;

            bool isVisible = false;
            if (value.GetType() == typeof(Visibility))
            {
                isVisible = (Visibility)value == Visibility.Visible;
            }
            return isVisible;
        }
    }
}
