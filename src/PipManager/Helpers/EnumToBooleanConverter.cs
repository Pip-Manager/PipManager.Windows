using System.Globalization;
using System.Windows.Data;

namespace PipManager.Helpers;

internal class EnumToBooleanConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (parameter is not string enumString)
        {
            throw new ArgumentException("ExceptionEnumToBooleanConverterParameterMustBeAnEnumName");
        }

        if (value != null && !Enum.IsDefined(typeof(Wpf.Ui.Appearance.ThemeType), value))
        {
            throw new ArgumentException("ExceptionEnumToBooleanConverterValueMustBeAnEnum");
        }

        var enumValue = Enum.Parse(typeof(Wpf.Ui.Appearance.ThemeType), enumString);

        return enumValue.Equals(value);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (parameter is not string enumString)
        {
            throw new ArgumentException("ExceptionEnumToBooleanConverterParameterMustBeAnEnumName");
        }

        return Enum.Parse(typeof(Wpf.Ui.Appearance.ThemeType), enumString);
    }
}