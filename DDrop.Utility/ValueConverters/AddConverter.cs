using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DDrop.Utility.ValueConverters
{
    public class AddConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] == DependencyProperty.UnsetValue && values[1] == DependencyProperty.UnsetValue)
                return string.Empty;

            if (values[1] == DependencyProperty.UnsetValue || values[1] == null)
            {
                if (values[0] == null)
                    return string.Empty;
                if (values[0] is int)
                    return int.Parse(values[0].ToString()).ToString(CultureInfo.InvariantCulture);
                if (values[0] is double)
                    return double.Parse(values[0].ToString()).ToString(CultureInfo.InvariantCulture);

                throw new InvalidOperationException("values type not supported");
            }

            if (values[0] == DependencyProperty.UnsetValue || values[0] == null)
            {
                if (values[1] == null)
                    return string.Empty;
                if (values[1] is int)
                    return int.Parse(values[1].ToString()).ToString(CultureInfo.InvariantCulture);
                if (values[1] is double)
                    return double.Parse(values[1].ToString()).ToString(CultureInfo.InvariantCulture);

                throw new InvalidOperationException("values type not supported");
            }

            if (values[0] == null)
                return string.Empty;

            if (values[0].GetType() != values[1].GetType())
                throw new InvalidOperationException("values must be of the same type");

            if (values[0] is int)
            {
                return ((int.Parse(values[0].ToString()) + int.Parse(values[1].ToString()))/2).ToString(CultureInfo.InvariantCulture);
            }

            if (values[0] is double)
            {
                return ((double.Parse(values[0].ToString()) + double.Parse(values[1].ToString()))/2).ToString(CultureInfo.InvariantCulture);
            }

            throw new InvalidOperationException("values type not supported");
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}