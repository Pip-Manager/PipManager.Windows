﻿using System.Globalization;
using System.Windows.Data;
using PipManager.Windows.Models.Package;

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

        if (currentPackageSource == "default" && currentParameter == "Official")
        {
            return true;
        }
        return currentPackageSource.Equals(currentParameter, StringComparison.OrdinalIgnoreCase);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null || parameter == null)
        {
            return "default";
        }

        var isChecked = (bool)value;
        var currentParameter = (string)parameter;

        if (isChecked)
        {
            return currentParameter == "Official" ? "default" : currentParameter;
        }
        return "default";
    }
}