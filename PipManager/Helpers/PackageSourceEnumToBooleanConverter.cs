using PipManager.Models;
using System.Globalization;
using System.Windows.Data;

namespace PipManager.Helpers;

internal class PackageSourceEnumToBooleanConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (parameter is not string enumString)
        {
            throw new ArgumentException("ExceptionPackageSourceTypeToBooleanConverterParameterMustBeAnEnumName");
        }

        if (value != null && !Enum.IsDefined(typeof(PackageSourceType), value))
        {
            throw new ArgumentException("ExceptionPackageSourceTypeToBooleanConverterValueMustBeAnEnum");
        }

        var enumValue = Enum.Parse(typeof(PackageSourceType), enumString);

        return enumValue.Equals(value);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (parameter is not string enumString)
        {
            throw new ArgumentException("ExceptionPackageSourceTypeToBooleanConverterParameterMustBeAnEnumName");
        }

        return Enum.Parse(typeof(PackageSourceType), enumString);
    }
}