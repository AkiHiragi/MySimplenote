using System;
using System.Globalization;
using System.Windows.Data;

namespace MySimplenote.Converters;

public class BooleanConverter:IValueConverter
{
    public static readonly BooleanConverter IsNotNull = new();
    
    public object? Convert(object?     value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value != null;
    }

    public object?  ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}