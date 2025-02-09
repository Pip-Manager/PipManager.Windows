using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Styling;

namespace PipManager.Desktop.Converters;

public class ThemeBoolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ThemeVariant themeVariant && parameter is string parameterString)
        {
            return themeVariant.ToString().Equals(parameterString, StringComparison.OrdinalIgnoreCase);
        }

        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not bool || parameter is not string parameterString) return null;
        return parameterString switch
        {
            "Light" => ThemeVariant.Light,
            "Dark" => ThemeVariant.Dark,
            _ => null
        };
    }
}