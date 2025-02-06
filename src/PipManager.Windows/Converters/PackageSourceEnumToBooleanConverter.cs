using System.Globalization;
using System.Windows.Data;

namespace PipManager.Windows.Converters;

internal class PackageSourceEnumToBooleanConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null || parameter == null)
        {
            return false;
        }

        var currentPackageSource = (string)value;
        var currentParameter = (string)parameter;

        if (currentPackageSource == "official" && currentParameter == "Official")
        {
            return true;
        }
        return currentPackageSource.Equals(currentParameter, StringComparison.OrdinalIgnoreCase);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null || parameter == null)
        {
            return "official";
        }

        var isChecked = (bool)value;
        var currentParameter = (string)parameter;

        if (isChecked)
        {
            return currentParameter == "Official" ? "official" : currentParameter;
        }
        return "official";
    }
}